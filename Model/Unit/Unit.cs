using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class Unit : GameObject
	{
		public EventHandler<EventArgs> OnLoad;
		public EventHandler<ValuedEventArgs<Unit>> OnUnload;
		public EventHandler<ValuedEventArgs<UnitConfiguration>> OnConfigurationChange;
		public EventHandler<MovementEventArgs> OnMove;
		public EventHandler<EventArgs> OnFire;
		public EventHandler<ValuedEventArgs<Tile>> OnRemove;
		public EventHandler<EventArgs> OnDestroy;
		public EventHandler<ValuedEventArgs<Army>> OnCapture;

		public bool Emplaced { get; private set; }

		public readonly Army Army;

		public int Id { get; private set; }
		public bool Fired { get; private set; }
		public bool Moved { get; private set; }
		public bool MovedMoreThanOneTile { get; private set; }
		public float RemainingMovement { get; private set; }
		public UnitStatus Status { get; private set; }
		public Tile Position { get; private set; }
		public Tile Target { get; set; }
		public Unit Passenger { get; private set; }
		public Unit Carrier { get; private set; }
		public Tile Evacuated { get; set; }

		List<Interaction> _Interactions = new List<Interaction>();
		UnitConfiguration _BaseConfiguration;
		bool _Dismounted;
		byte _PrimaryAmmunition;
		byte _SecondaryAmmunition;

		bool[] _ReconDirections = new bool[Enum.GetValues(typeof(Direction)).Length];

		public Deployment Deployment
		{
			get
			{
				return Army.Deployments.Find(i => i.Units.Contains(this));
			}
		}
		public UnitConfiguration Configuration
		{
			get
			{
				return _Dismounted ? _BaseConfiguration.DismountAs : _BaseConfiguration;
			}
		}
		public IEnumerable<Interaction> Interactions
		{
			get
			{
				return _Interactions;
			}
		}

		public Unit(Army Army, UnitConfiguration UnitConfiguration, IdGenerator IdGenerator)
		{
			this.Army = Army;
			_BaseConfiguration = UnitConfiguration;
			if (UnitConfiguration.PrimaryWeapon.Ammunition > 0)
				_PrimaryAmmunition = UnitConfiguration.PrimaryWeapon.Ammunition;
			if (UnitConfiguration.SecondaryWeapon.Ammunition > 0)
				_SecondaryAmmunition = UnitConfiguration.SecondaryWeapon.Ammunition;
			Id = IdGenerator.GenerateId();
		}

		public byte GetAmmunition(bool Secondary)
		{
			return Secondary ? _SecondaryAmmunition : _PrimaryAmmunition;
		}

		public void UseAmmunition(bool Secondary)
		{
			if (Secondary) _SecondaryAmmunition--;
			else _PrimaryAmmunition--;
		}

		public OrderInvalidReason CanBeAttackedBy(Army Army, AttackMethod AttackMethod, bool IgnoreConcealment = false)
		{
			IgnoreConcealment |= AttackMethod == AttackMethod.INDIRECT_FIRE;
			if (Position == null) return OrderInvalidReason.ILLEGAL;

			if (AttackMethod == AttackMethod.MINEFIELD)
			{
				if (Configuration.ImmuneToMines) return OrderInvalidReason.TARGET_IMMUNE;
				if (Position.Units.Where(i => i.Configuration.UnitClass == UnitClass.MINEFIELD)
					== HasInteraction<ClearMinefieldInteraction>(i => true)?.Object)
					return OrderInvalidReason.TARGET_IMMUNE;
				if (!Configuration.IsNeutral()) return OrderInvalidReason.NONE;
			}

			if (Configuration.IsAircraft() && AttackMethod != AttackMethod.ANTI_AIRCRAFT)
				return OrderInvalidReason.TARGET_IMMUNE;

			if (Army.Configuration.Team == this.Army.Configuration.Team
				|| (Configuration.IsNeutral() && !Configuration.MustBeAttackedAlone()))
				return OrderInvalidReason.TARGET_TEAM;
			if (Position == null) return OrderInvalidReason.ILLEGAL;
			if (Configuration.MustBeAttackedAlone())
			{
				if (Position.Units.Any(
					i => i != this && i.CanBeAttackedBy(Army, AttackMethod) == OrderInvalidReason.NONE))
					return OrderInvalidReason.TARGET_COVERED;
			}
			if (!IgnoreConcealment && !Army.SightFinder.IsSighted(this))
				return OrderInvalidReason.TARGET_CONCEALED;
			if (Carrier != null || Position.Units.Any(i => i.Covers(this))) return OrderInvalidReason.TARGET_COVERED;
			return OrderInvalidReason.NONE;
		}

		public bool Covers(Unit Unit)
		{
			return this != Unit && Army == Unit.Army && Configuration.Covers(Unit.Configuration);
		}

		public bool CanExitDirection(Direction Direction)
		{
			if (Position == null) return false;
			return (Direction == Direction.WEST || Direction == Direction.NORTH || Direction == Direction.EAST
					|| Direction == Direction.SOUTH) && Position.OnEdge(Direction);
		}

		public OrderInvalidReason CanEnter(Tile Tile, bool Terminal = false, bool IgnoreEnemyUnits = false)
		{
			if (!IgnoreEnemyUnits && !Configuration.IsAircraft() && Tile.GetUnitBlockType() == BlockType.STANDARD
				&& Tile.Units.Any(i => !i.Configuration.IsNeutral() && !i.Configuration.IsAircraft() && i.Army != Army))
				return OrderInvalidReason.TILE_ENEMY_OCCUPIED;
			if (Configuration.IsStackUnique() && Tile.Units.Any(i => i != this && i.Configuration.IsStackUnique()))
				return OrderInvalidReason.UNIT_UNIQUE;
			if (Configuration.UnitClass == UnitClass.FORT && Tile.Rules.Watery)
				return OrderInvalidReason.UNIT_EMPLACE_TERRAIN;
			if (Terminal
				&& Tile.GetStackSize() + GetStackSize() > Army.Configuration.Faction.StackLimit
				&& !Tile.Units.Contains(this))
				return OrderInvalidReason.UNIT_STACK_LIMIT;
			return OrderInvalidReason.NONE;
		}

		public OrderInvalidReason CanMove(bool Combat)
		{
			if (Position == null || Fired || Status != UnitStatus.ACTIVE || Carrier != null)
				return OrderInvalidReason.UNIT_NO_MOVE;
			if (RemainingMovement > 0)
			{
				if (Combat)
				{
					if (Configuration.CanOverrun) return OrderInvalidReason.NONE;
					if (Configuration.CanCloseAssault)
						return MovedMoreThanOneTile ? OrderInvalidReason.UNIT_NO_MOVE : OrderInvalidReason.NONE;
					return OrderInvalidReason.UNIT_NO_MOVE;
				}
				return OrderInvalidReason.NONE;
			}
			return OrderInvalidReason.UNIT_NO_MOVE;
		}

		public OrderInvalidReason CanMove(bool Vehicle, bool Combat)
		{
			if (Vehicle != Configuration.IsVehicle) return OrderInvalidReason.UNIT_NO_MOVE;
			return CanMove(Combat);
		}

		public OrderInvalidReason CanAttack(AttackMethod AttackMethod)
		{
			if (Position == null || Fired || MovedMoreThanOneTile || Carrier != null)
				return OrderInvalidReason.UNIT_NO_ACTION;
			if (Status != UnitStatus.ACTIVE)
			{
				if (Configuration.UnitClass == UnitClass.MINEFIELD && AttackMethod == AttackMethod.MINEFIELD)
					return OrderInvalidReason.NONE;
				return OrderInvalidReason.UNIT_NO_ACTION;
			}
			if (AttackMethod != AttackMethod.CLOSE_ASSAULT && Moved) return OrderInvalidReason.UNIT_NO_ACTION;
			if (MustMove()) return OrderInvalidReason.UNIT_MUST_MOVE;
			return Configuration.CanAttack(AttackMethod);
		}

		public OrderInvalidReason CanAttack(
			AttackMethod AttackMethod, bool EnemyArmored, LineOfSight LineOfSight, bool UseSecondaryWeapon)
		{
			var r = CanAttack(AttackMethod);
			if (r != OrderInvalidReason.NONE) return r;

			if (Configuration.GetWeapon(UseSecondaryWeapon).Ammunition > 0 && GetAmmunition(UseSecondaryWeapon) == 0)
				return OrderInvalidReason.UNIT_NO_AMMUNITION;

			if (AttackMethod == AttackMethod.DIRECT_FIRE)
			{
				return Configuration.CanDirectFireAt(EnemyArmored, LineOfSight, UseSecondaryWeapon);
			}
			if (AttackMethod == AttackMethod.INDIRECT_FIRE)
			{
				r = Configuration.CanIndirectFireAt(LineOfSight, UseSecondaryWeapon);
				if (r != OrderInvalidReason.NONE) return r;

				if (Configuration.CanDirectFireAt(
					EnemyArmored, LineOfSight, UseSecondaryWeapon) != OrderInvalidReason.NONE)
				{
					if (Army.SightFinder.HasTileSightLevel(LineOfSight.Final, TileSightLevel.SOFT_SPOTTED))
						return OrderInvalidReason.NONE;
					return OrderInvalidReason.ATTACK_NO_SPOTTER;
				}
				return OrderInvalidReason.NONE;
			}
			if (AttackMethod == AttackMethod.OVERRUN) return Configuration.CanOverrunAt(EnemyArmored);
			if (AttackMethod == AttackMethod.CLOSE_ASSAULT)
			{
				if (!Configuration.CanCloseAssault) return OrderInvalidReason.UNIT_NO_ATTACK;
			}
			if (AttackMethod == AttackMethod.AIR)
				return Configuration.CanAirAttackAt(EnemyArmored, UseSecondaryWeapon);
			if (AttackMethod == AttackMethod.ANTI_AIRCRAFT)
				return Configuration.CanAntiAircraftAt(EnemyArmored, LineOfSight, UseSecondaryWeapon);

			return OrderInvalidReason.NONE;
		}

		public void Capture(Army Army)
		{
			if (Status != UnitStatus.DESTROYED && Status != UnitStatus.CAPTURED)
			{
				Status = UnitStatus.CAPTURED;
				if (OnCapture != null) OnCapture(this, new ValuedEventArgs<Army>(Army));
				CancelInteractions();
				Remove();
			}
		}

		public void HandleCombatResult(CombatResult CombatResult, AttackMethod AttackMethod, Army AttackingArmy)
		{
			if (Position == null) return;
			if (Passenger != null) Passenger.HandleCombatResult(CombatResult, AttackMethod, AttackingArmy);
			foreach (var unit in Position.Units.Where(i => Covers(i)).ToList())
				unit.HandleCombatResult(CombatResult, AttackMethod, AttackingArmy);
			switch (CombatResult)
			{
				case CombatResult.MISS:
					return;
				case CombatResult.DESTROY:
					if (AttackMethod == AttackMethod.CLOSE_ASSAULT && Configuration.CloseAssaultCapture)
						Capture(AttackingArmy);
					else
					{
						Status = UnitStatus.DESTROYED;
						if (OnDestroy != null) OnDestroy(this, EventArgs.Empty);
						CancelInteractions();
						Remove();
					}
					return;
				case CombatResult.DAMAGE:
					Status = UnitStatus.DAMAGED;
					return;
				case CombatResult.DISRUPT:
					Status = UnitStatus.DISRUPTED;
					if (Configuration.UnloadsWhenDisrupted && Passenger != null) Unload(false);
					return;
				case CombatResult.DOUBLE_DISRUPT:
					if (Status == UnitStatus.DISRUPTED)
						HandleCombatResult(CombatResult.DESTROY, AttackMethod, AttackingArmy);
					else
					{
						Status = UnitStatus.DISRUPTED;
						if (Configuration.UnloadsWhenDisrupted && Passenger != null) Unload(false);
					}
					return;
			}
		}

		public void Remove()
		{
			Position.Exit(this);
			Position.UpdateControl();
			var position = Position;
			Position = null;
			if (OnRemove != null) OnRemove(this, new ValuedEventArgs<Tile>(position));
		}

		public void Place(Tile Tile, Path<Tile> Path = null)
		{
			if (Passenger != null) Passenger.Place(Tile, Path);
			if (Position != null) Position.Exit(this);
			Position = Tile;
			Position.Enter(this);
			Position.UpdateControl();

			if (OnMove != null) OnMove(this, new MovementEventArgs(Tile, Path, Carrier));
		}

		public void MoveTo(Tile Tile, Path<Tile> Path)
		{
			if (Tile == Position && (Path == null || Path.Count < 2)) return;
			foreach (Tile t in Path.Nodes) t.Control(this);

			if (!Configuration.HasUnlimitedMovement())
			{
				var movement = (float)Path.Distance;
				RemainingMovement -= movement;
				MovedMoreThanOneTile = Path.Count > 2 || Moved;
				Moved = true;
			}
			else
			{
				RemainingMovement = 0;
			}
			Place(Tile, Path);
		}

		public bool MustMove()
		{
			return Deployment?.UnitMustMove(this) ?? false;
		}

		public OrderInvalidReason CanDismount()
		{
			if (Carrier != null || Status != UnitStatus.ACTIVE || Moved || Fired)
				return OrderInvalidReason.UNIT_NO_ACTION;
			if (_BaseConfiguration.DismountAs == null) return OrderInvalidReason.UNIT_NO_DISMOUNT;
			if (MustMove()) return OrderInvalidReason.UNIT_MUST_MOVE;
			return OrderInvalidReason.NONE;
		}

		public OrderInvalidReason CanMount(bool Deployment)
		{
			if (!Deployment && !Configuration.CanRemount) return OrderInvalidReason.UNIT_NO_REMOUNT;
			return CanDismount();
		}

		public OrderInvalidReason CanLoad(Unit Unit, bool UseMovement)
		{
			if (UseMovement)
			{
				if (Unit.Moved || RemainingMovement < GetLoadCost(Unit) || Unit.Fired || Status != UnitStatus.ACTIVE)
					return OrderInvalidReason.UNIT_NO_MOVE;
			}
			if (Unit.Army != Army) return OrderInvalidReason.TARGET_TEAM;
			if (Unit.Position != Position) return OrderInvalidReason.ILLEGAL;
			if (Passenger != null) return OrderInvalidReason.UNIT_CARRYING;
			if (Unit.Carrier != null) return OrderInvalidReason.TARGET_CARRIED;
			if (MustMove()) return OrderInvalidReason.UNIT_MUST_MOVE;
			if (Position != null && Position.Rules.Watery && !Configuration.CanCarryInWater)
				return OrderInvalidReason.UNIT_NO_CARRY_IN_WATER;

			return Configuration.CanLoad(Unit.Configuration);
		}

		public OrderInvalidReason CanUnload(bool UseMovement)
		{
			if (Passenger == null) return OrderInvalidReason.UNIT_NO_PASSENGER;
			if (UseMovement)
			{
				if (Status != UnitStatus.ACTIVE || RemainingMovement < GetLoadCost(Passenger))
					return OrderInvalidReason.UNIT_NO_MOVE;
			}
			if (Position != null)
			{
				var r = Passenger.CanEnter(Position);
				if (r != OrderInvalidReason.NONE) return r;

				if (Position.GetStackSize() + Passenger.Configuration.GetStackSize()
					> Army.Configuration.Faction.StackLimit)
					return OrderInvalidReason.UNIT_STACK_LIMIT;
			}
			if (MustMove()) return OrderInvalidReason.UNIT_MUST_MOVE;
			return OrderInvalidReason.NONE;
		}

		public OrderInvalidReason CanClearMinefield()
		{
			if (Carrier != null || Status != UnitStatus.ACTIVE || Moved || Fired)
				return OrderInvalidReason.UNIT_NO_ACTION;
			if (MustMove()) return OrderInvalidReason.UNIT_MUST_MOVE;
			if (!Configuration.CanClearMines) return OrderInvalidReason.UNIT_NO_ENGINEER;
			return OrderInvalidReason.NONE;
		}

		public OrderInvalidReason CanPlaceMinefield()
		{
			if (Carrier != null || Status != UnitStatus.ACTIVE || Moved || Fired)
				return OrderInvalidReason.UNIT_NO_ACTION;
			if (MustMove()) return OrderInvalidReason.UNIT_MUST_MOVE;
			if (!Configuration.CanPlaceMines) return OrderInvalidReason.UNIT_NO_ENGINEER;
			return OrderInvalidReason.NONE;
		}

		public OrderInvalidReason CanPlaceBridge()
		{
			if (Carrier != null || Status != UnitStatus.ACTIVE || Moved || Fired)
				return OrderInvalidReason.UNIT_NO_ACTION;
			if (MustMove()) return OrderInvalidReason.UNIT_MUST_MOVE;
			if (!Configuration.CanPlaceBridges) return OrderInvalidReason.UNIT_NO_ENGINEER;
			return OrderInvalidReason.NONE;
		}

		public OrderInvalidReason CanBeEmplaced()
		{
			if (Position == null) return OrderInvalidReason.ILLEGAL;
			if (Carrier != null || Status != UnitStatus.ACTIVE || Moved || Fired)
				return OrderInvalidReason.UNIT_NO_ACTION;
			if (MustMove()) return OrderInvalidReason.UNIT_MUST_MOVE;

			if (Configuration.UnitClass == UnitClass.BRIDGE && !Position.Rules.Bridgeable)
				return OrderInvalidReason.UNIT_EMPLACE_TERRAIN;
			if (!Configuration.IsEmplaceable() || Emplaced) return OrderInvalidReason.TARGET_NOT_EMPLACEABLE;
			return OrderInvalidReason.NONE;
		}

		public bool CanSight()
		{
			return Status == UnitStatus.ACTIVE && Position != null && Carrier == null && !Configuration.IsEmplaceable();
		}

		public bool CanSpot()
		{
			return Status == UnitStatus.ACTIVE && Position != null && Carrier == null && Configuration.CanSpot;
		}

		public void Dismount(bool UseMovement)
		{
			_Dismounted = true;
			if (OnConfigurationChange != null)
				OnConfigurationChange(this, new ValuedEventArgs<UnitConfiguration>(_BaseConfiguration));
			if (UseMovement) Halt();
		}

		public void Mount(bool UseMovement)
		{
			_Dismounted = false;
			if (OnConfigurationChange != null)
				OnConfigurationChange(this, new ValuedEventArgs<UnitConfiguration>(_BaseConfiguration.DismountAs));
			if (UseMovement) Halt();
		}

		public void Load(Unit Unit, bool UseMovement)
		{
			Passenger = Unit;
			Unit.Carrier = this;

			if (UseMovement)
			{
				if (Moved) Halt();
				else
				{
					Moved = true;
					RemainingMovement -= GetLoadCost(Unit);
				}
				Passenger.Halt();
			}

			if (OnLoad != null) OnLoad(this, EventArgs.Empty);
		}

		public void Unload(bool UseMovement)
		{
			if (UseMovement)
			{
				if (Moved) Halt();
				else
				{
					Moved = true;
					RemainingMovement -= GetLoadCost(Passenger);
				}
				Passenger.Halt();
			}

			Unit passenger = Passenger;
			Passenger.Carrier = null;
			Passenger = null;

			if (OnUnload != null) OnUnload(this, new ValuedEventArgs<Unit>(passenger));
		}

		public byte GetLoadCost(Unit Unit)
		{
			if (Unit.Configuration.UnitWeight == UnitWeight.HEAVY) return Configuration.Movement;
			return (byte)((Configuration.Movement + 1) / 2);
		}

		public int GetStackSize()
		{
			return Carrier == null ? Configuration.GetStackSize() : 0;
		}

		public LineOfSight GetLineOfSight(Tile Tile)
		{
			return GetLineOfSight(Position, Tile);
		}

		public LineOfSight GetLineOfSight(Tile From, Tile To)
		{
			if (From == null || To == null) return null;
			return new LineOfSight(From, To);
		}

		public Path<Tile> GetPathTo(Tile Tile, bool Combat)
		{
			if (Position == null) return null;
			return GetPathTo(Position, Tile, Combat);
		}

		public Path<Tile> GetPathTo(Tile From, Tile Tile, bool Combat)
		{
			return new Path<Tile>(
				From,
				Tile,
				i => true,
				(i, j) => i.Rules.GetMoveCost(this, j, !Combat).Cost,
				(i, j) => i.HeuristicDistanceTo(j),
				i => i.Neighbors(),
				(i, j) => i == j);
		}

		public IEnumerable<Tile> GetFieldOfDeployment(IEnumerable<Tile> Tiles)
		{
			var deployment = Deployment;
			if (deployment != null && deployment is PositionalDeployment)
				return Tiles.Where(
					i => ((PositionalDeployment)deployment).Validate(this, i) == OrderInvalidReason.NONE);
			return Enumerable.Empty<Tile>();
		}

		public IEnumerable<LineOfSight> GetFieldOfSight(AttackMethod AttackMethod)
		{
			return GetFieldOfSight(AttackMethod, Position);
		}

		public IEnumerable<LineOfSight> GetFieldOfSight(AttackMethod AttackMethod, Tile Tile)
		{
			if (Configuration.CanAttack(AttackMethod) == OrderInvalidReason.NONE)
			{
				return GetFieldOfSight(
					Configuration.GetRange(AttackMethod, false), Tile, AttackMethod);
			}
			return Enumerable.Empty<LineOfSight>();
		}

		public IEnumerable<LineOfSight> GetFieldOfSight(int Range, Tile Tile, AttackMethod AttackMethod)
		{
			return GetLinesOfSight(Range, Tile)
				.Where(i => CanAttack(AttackMethod, false, i, false) == OrderInvalidReason.NONE);
		}

		public IEnumerable<LineOfSight> GetFieldOfSight(int Range, Tile Tile)
		{
			if (Configuration.IsAircraft()) return GetLinesOfSight(Range, Tile);
			return GetLinesOfSight(Range, Tile).Where(i => i.Validate() == NoLineOfSightReason.NONE);
		}

		public IEnumerable<LineOfSight> GetLinesOfSight(int Range, Tile Tile)
		{
			if (Tile != null)
			{
				for (int i = -Range; i <= Range; ++i)
				{
					for (int j = Math.Max(-Range, -(i + Range)); j <= Math.Min(Range, Range - i); ++j)
					{
						HexCoordinate p = Tile.HexCoordinate;
						Coordinate c = new HexCoordinate(p.X + i, p.Y + j, p.Z - i - j).ToCoordinate();
						if (c.X >= 0 && c.X < Tile.Map.Tiles.GetLength(0)
							&& c.Y >= 0 && c.Y < Tile.Map.Tiles.GetLength(1))
						{
							yield return GetLineOfSight(Tile, Tile.Map.Tiles[c.X, c.Y]);
						}
					}
				}
			}
		}

		public IEnumerable<Tuple<Tile, Tile, double>> GetFieldOfMovement(bool Combat)
		{
			if (Position == null) return null;
			if (CanMove(Combat) != OrderInvalidReason.NONE) return Enumerable.Empty<Tuple<Tile, Tile, double>>();

			var adjacent =
				Position.NeighborTiles
		   			.Where(i => i != null && Position.Rules.CanMove(this, i, !Combat, false))
					.Select(i => new Tuple<Tile, Tile, double>(
							 i, Position, Position.Rules.GetMoveCost(this, i, !Combat).Cost));
			if (Combat && Configuration.CanCloseAssault)
				return adjacent;

			var fullMovement = new Field<Tile>(
				Position,
				RemainingMovement,
				(i, j) => i.Rules.GetMoveCost(this, j, !Combat).Cost)
					.GetReachableNodes();

			if (!Moved)
				return fullMovement.Concat(adjacent.Where(i => !fullMovement.Any(j => i.Item1 == j.Item1)));
			return fullMovement;
		}

		public void Recon(Direction Direction)
		{
			_ReconDirections[(int)Direction] = true;
		}

		public bool HasRecon(Direction Direction)
		{
			return _ReconDirections[(int)Direction];
		}

		public void AddInteraction(Interaction Interaction)
		{
			_Interactions.Add(Interaction);
		}

		public void CancelInteractions()
		{
			_Interactions.ToList().ForEach(CancelInteraction);
		}

		public void CancelInteraction(Interaction Interaction)
		{
			_Interactions.Remove(Interaction);
		}

		public T HasInteraction<T>(Func<T, bool> Predicate)
		{
			return (T)_Interactions.FirstOrDefault(i => i is T && Predicate((T)i));
		}

		public void DoInteractions()
		{
			_Interactions.ForEach(DoInteraction);
		}

		public void DoInteraction(Interaction Interaction)
		{
			if (Interaction != null && !Interaction.Apply(this)) Interaction.Cancel();
		}

		public void Emplace(bool Emplaced)
		{
			this.Emplaced = Emplaced && Configuration.IsEmplaceable();
		}

		public void Fire(Tile Tile, bool UseSecondary)
		{
			Fired = true;
			if (Tile != Target) Target = null;
			if (Configuration.GetWeapon(UseSecondary).Ammunition > 0) UseAmmunition(UseSecondary);
			CancelInteractions();
			if (OnFire != null) OnFire(this, EventArgs.Empty);
		}

		public void Halt()
		{
			Moved = true;
			MovedMoreThanOneTile = true;
			RemainingMovement = 0;
		}

		public void Reset()
		{
			Fired = false;
			Moved = false;
			MovedMoreThanOneTile = false;
			RemainingMovement = Configuration.GetMaxMovement(Army.Match.Scenario.Environment);
			if (Status == UnitStatus.DISRUPTED) Status = UnitStatus.ACTIVE;
			if (Status == UnitStatus.DAMAGED && Position != null)
			{
				Remove();
				CancelInteractions();
			}

			DoInteractions();
		}

		public float GetPointValue()
		{
			return Configuration.GetPointValue(Army.Configuration.Faction.HalfPriceTrucks);
		}

		public override string ToString()
		{
			return string.Format("[Unit: Id={0}, Configuration={1}]", Id, Configuration);
		}
	}
}
