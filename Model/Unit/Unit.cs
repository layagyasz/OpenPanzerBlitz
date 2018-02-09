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
		public EventHandler<EventArgs> OnConfigurationChange;
		public EventHandler<MovementEventArgs> OnMove;
		public EventHandler<EventArgs> OnFire;
		public EventHandler<EventArgs> OnRemove;
		public EventHandler<EventArgs> OnDestroy;

		public bool Deployed { get; set; }

		public readonly Army Army;

		public int Id { get; private set; }
		public bool Fired { get; private set; }
		public bool Moved { get; private set; }
		public bool MovedMoreThanOneTile { get; private set; }
		public float RemainingMovement { get; private set; }
		public UnitStatus Status { get; private set; }
		public Tile Position { get; private set; }
		public Unit Passenger { get; private set; }
		public Unit Carrier { get; private set; }
		public Direction Evacuated { get; set; } = Direction.NONE;
		public Interaction Interaction { get; private set; }

		UnitConfiguration _BaseConfiguration;
		bool _Dismounted;

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

		public Unit(Army Army, UnitConfiguration UnitConfiguration, IdGenerator IdGenerator)
		{
			this.Army = Army;
			_BaseConfiguration = UnitConfiguration;
			Id = IdGenerator.GenerateId();
		}

		public OrderInvalidReason CanBeAttackedBy(Army Army)
		{
			if (Army.Configuration.Team == this.Army.Configuration.Team || Configuration.IsNeutral())
				return OrderInvalidReason.TARGET_TEAM;
			if (Position == null) return OrderInvalidReason.ILLEGAL;
			if (Configuration.UnitClass == UnitClass.FORT)
			{
				if (Position.Units.Any(
					i => i != this && i.Army == this.Army && i.CanBeAttackedBy(Army) == OrderInvalidReason.NONE))
					return OrderInvalidReason.NONE;
			}
			if (!Army.CanSeeUnit(this)) return OrderInvalidReason.TARGET_CONCEALED;
			if (Carrier != null) return OrderInvalidReason.UNIT_NO_ACTION;
			return OrderInvalidReason.NONE;
		}

		public bool CanExitDirection(Direction Direction)
		{
			if (Position == null) return false;
			return (Direction == Direction.WEST || Direction == Direction.NORTH || Direction == Direction.EAST
					|| Direction == Direction.SOUTH) && Position.OnEdge(Direction);
		}

		public OrderInvalidReason CanEnter(Tile Tile, bool Terminal = false, bool IgnoreEnemyUnits = false)
		{
			if (!IgnoreEnemyUnits && Tile.GetUnitBlockType() == BlockType.STANDARD
				&& Tile.Units.Any(i => !i.Configuration.IsNeutral() && i.Army != Army))
				return OrderInvalidReason.TILE_ENEMY_OCCUPIED;
			if (Configuration.IsStackUnique() && Tile.Units.Any(i => i != this && i.Configuration.IsStackUnique()))
				return OrderInvalidReason.UNIT_UNIQUE;
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
			if (Position == null || Fired || MovedMoreThanOneTile || Status != UnitStatus.ACTIVE || Carrier != null)
				return OrderInvalidReason.UNIT_NO_ACTION;
			if (AttackMethod != AttackMethod.CLOSE_ASSAULT && Moved) return OrderInvalidReason.UNIT_NO_ACTION;
			if (MustMove()) return OrderInvalidReason.UNIT_MUST_MOVE;
			return Configuration.CanAttack(AttackMethod);
		}

		public OrderInvalidReason CanAttack(AttackMethod AttackMethod, bool EnemyArmored, LineOfSight LineOfSight)
		{
			OrderInvalidReason r = CanAttack(AttackMethod);
			if (r != OrderInvalidReason.NONE) return r;

			if (AttackMethod == AttackMethod.NORMAL_FIRE && LineOfSight.Validate() != NoLineOfSightReason.NONE)
				return OrderInvalidReason.ATTACK_NO_LOS;

			if (AttackMethod == AttackMethod.NORMAL_FIRE)
			{
				r = Configuration.CanDirectFireAt(EnemyArmored, LineOfSight);
				if (r != OrderInvalidReason.UNIT_NO_ATTACK)
					return r;
				r = Configuration.CanIndirectFireAt(LineOfSight);
				if (r != OrderInvalidReason.NONE) return r;
				if (!Army.CanIndirectFireAtTile(LineOfSight.Final))
					return OrderInvalidReason.ATTACK_NO_SPOTTER;
				return OrderInvalidReason.NONE;
			}
			if (AttackMethod == AttackMethod.OVERRUN) return Configuration.CanOverrunAt(EnemyArmored);
			return Configuration.CanCloseAssaultAt(LineOfSight);
		}

		public void HandleCombatResult(CombatResult CombatResult)
		{
			if (Passenger != null) Passenger.HandleCombatResult(CombatResult);
			switch (CombatResult)
			{
				case CombatResult.MISS:
					return;
				case CombatResult.DESTROY:
					Status = UnitStatus.DESTROYED;
					if (OnDestroy != null) OnDestroy(this, EventArgs.Empty);
					Remove();
					return;
				case CombatResult.DISRUPT:
					Status = UnitStatus.DISRUPTED;
					return;
				case CombatResult.DOUBLE_DISRUPT:
					if (Status == UnitStatus.DISRUPTED)
					{
						Status = UnitStatus.DESTROYED;
						if (OnDestroy != null) OnDestroy(this, EventArgs.Empty);
						Remove();
					}
					else Status = UnitStatus.DISRUPTED;
					return;
			}
		}

		public void Remove()
		{
			Position.Exit(this);
			Position = null;
			if (OnRemove != null) OnRemove(this, EventArgs.Empty);
		}

		public void Place(Tile Tile, Path<Tile> Path = null)
		{
			if (Passenger != null) Passenger.Place(Tile, Path);
			if (Position != null) Position.Exit(this);
			Position = Tile;
			Position.Enter(this);

			if (OnMove != null) OnMove(this, new MovementEventArgs(Tile, Path, Carrier));
		}

		public void MoveTo(Tile Tile, Path<Tile> Path)
		{
			if (Tile == Position) return;
			foreach (Tile t in Path.Nodes) t.Control(this);

			float movement = (float)Path.Distance;
			RemainingMovement -= movement;
			MovedMoreThanOneTile = Path.Count > 2 || Moved;
			Moved = true;
			Place(Tile, Path);

		}

		public bool MustMove()
		{
			return Deployment.UnitMustMove(this);
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

		public OrderInvalidReason CanLoad(Unit Unit)
		{
			if (Unit.Moved || Moved || Unit.Fired || Status != UnitStatus.ACTIVE)
				return OrderInvalidReason.UNIT_NO_MOVE;
			if (Unit.Army != Army) return OrderInvalidReason.TARGET_TEAM;
			if (Unit.Position != Position) return OrderInvalidReason.ILLEGAL;
			if (Passenger != null) return OrderInvalidReason.UNIT_CARRYING;
			if (Unit.Carrier != null) return OrderInvalidReason.TARGET_CARRIED;
			if (MustMove()) return OrderInvalidReason.UNIT_MUST_MOVE;
			if (Position != null && Position.RulesCalculator.Water && !Configuration.CanCarryInWater)
				return OrderInvalidReason.UNIT_NO_CARRY_IN_WATER;

			return Configuration.CanLoad(Unit.Configuration);
		}

		public OrderInvalidReason CanUnload()
		{
			if (Status != UnitStatus.ACTIVE) return OrderInvalidReason.UNIT_NO_MOVE;
			if (Passenger == null) return OrderInvalidReason.UNIT_NO_PASSENGER;
			if (Position != null)
			{
				OrderInvalidReason r = Passenger.CanEnter(Position);
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
			if (!Configuration.IsEngineer) return OrderInvalidReason.UNIT_NO_ENGINEER;
			return OrderInvalidReason.NONE;
		}

		public bool IsSolitary()
		{
			return Position.Units.Count() == 1 || Position.Units.All(i => i == this || i == Passenger);
		}

		public void Dismount(bool UseMovement)
		{
			_Dismounted = true;
			if (OnConfigurationChange != null) OnConfigurationChange(this, EventArgs.Empty);
			if (UseMovement) Halt();
		}

		public void Mount(bool UseMovement)
		{
			_Dismounted = false;
			if (OnConfigurationChange != null) OnConfigurationChange(this, EventArgs.Empty);
			if (UseMovement) Halt();
		}

		public void Load(Unit Unit, bool UseMovement)
		{
			Passenger = Unit;
			Unit.Carrier = this;

			if (UseMovement)
			{
				Halt();
				Passenger.Halt();
			}

			if (OnLoad != null) OnLoad(this, EventArgs.Empty);
		}

		public void Unload(bool UseMovement)
		{
			if (UseMovement)
			{
				Halt();
				Passenger.Halt();
			}

			Unit passenger = Passenger;
			Passenger.Carrier = null;
			Passenger = null;

			if (OnUnload != null) OnUnload(this, new ValuedEventArgs<Unit>(passenger));
		}

		public int GetStackSize()
		{
			return Carrier == null ? Configuration.GetStackSize() : 0;
		}

		public LineOfSight GetLineOfSight(Tile Tile)
		{
			if (Position == null) return null;
			return new LineOfSight(Position, Tile);
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
				(i, j) => i.RulesCalculator.GetMoveCost(this, j, !Combat).Cost,
				(i, j) => i.HeuristicDistanceTo(j),
				i => i.Neighbors(),
				(i, j) => i == j);
		}

		public IEnumerable<Tile> GetFieldOfDeployment(IEnumerable<Tile> Tiles)
		{
			if (Deployment is PositionalDeployment)
				return Tiles.Where(
					i => ((PositionalDeployment)Deployment).Validate(this, i) == OrderInvalidReason.NONE);
			return Enumerable.Empty<Tile>();
		}

		public IEnumerable<Tuple<LineOfSight, bool>> GetFieldOfSight(AttackMethod AttackMethod)
		{
			if (Position != null && CanAttack(AttackMethod) == OrderInvalidReason.NONE)
			{
				foreach (LineOfSight l in new Field<Tile>(Position, Configuration.GetRange(AttackMethod), (i, j) => 1)
						 .GetReachableNodes()
						 .Select(i => GetLineOfSight(i.Item1))
						 .Where(i => i.Final != Position))
				{
					if (l.Validate() == NoLineOfSightReason.NONE) yield return new Tuple<LineOfSight, bool>(l, true);
					else if (CanAttack(AttackMethod, false, l) == OrderInvalidReason.NONE)
						yield return new Tuple<LineOfSight, bool>(l, false);
				}
			}
		}

		public IEnumerable<Tuple<Tile, Tile, double>> GetFieldOfMovement(bool Combat)
		{
			if (Position == null) return null;
			if (CanMove(Combat) != OrderInvalidReason.NONE) return Enumerable.Empty<Tuple<Tile, Tile, double>>();

			IEnumerable<Tuple<Tile, Tile, double>> adjacent =
				Position.NeighborTiles
		   			.Where(i => i != null && Position.RulesCalculator.CanMove(this, i, !Combat, false))
					.Select(i => new Tuple<Tile, Tile, double>(
							 i, Position, Position.RulesCalculator.GetMoveCost(this, i, !Combat).Cost));
			if (Combat && Configuration.CanCloseAssault)
				return adjacent;

			IEnumerable<Tuple<Tile, Tile, double>> fullMovement = new Field<Tile>(
				Position,
				RemainingMovement,
				(i, j) => i.RulesCalculator.GetMoveCost(this, j, !Combat).Cost)
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

		public void SetInteraction(Interaction Interaction)
		{
			if (Interaction != null) CancelInteraction();
			this.Interaction = Interaction;
		}

		public void CancelInteraction()
		{
			if (Interaction != null)
			{
				Interaction i = Interaction;
				Interaction = null;
				i.Cancel();
			}
		}

		public T HasInteraction<T>(Func<T, bool> Predicate)
		{
			if (Interaction is T) return Predicate((T)Interaction) ? (T)Interaction : default(T);
			return default(T);
		}

		public void DoInteraction()
		{
			if (Interaction.Master == this && !Interaction.Apply()) Interaction.Cancel();
		}

		public void Fire()
		{
			Fired = true;
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
