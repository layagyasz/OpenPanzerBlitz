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

		public readonly Army Army;

		int _Id;
		bool _Deployed;
		UnitConfiguration _BaseConfiguration;
		bool _Dismounted;
		float _RemainingMovement;
		bool _Fired;
		bool _Moved;
		bool _MovedMoreThanOneTile;
		UnitStatus _Status;

		Tile _Position;

		Unit _Passenger;
		Unit _Carrier;

		bool[] _ReconDirections = new bool[Enum.GetValues(typeof(Direction)).Length];
		public Direction Evacuated { get; set; } = Direction.NONE;

		public int Id
		{
			get
			{
				return _Id;
			}
		}
		public Deployment Deployment
		{
			get
			{
				return Army.Deployments.Find(i => i.Units.Contains(this));
			}
		}
		public bool Deployed
		{
			get
			{
				return _Deployed;
			}
			set
			{
				_Deployed = value;
			}
		}
		public UnitConfiguration Configuration
		{
			get
			{
				return _Dismounted ? _BaseConfiguration.DismountAs : _BaseConfiguration;
			}
		}
		public float RemainingMovement
		{
			get
			{
				return _RemainingMovement;
			}
		}
		public bool Fired
		{
			get
			{
				return _Fired;
			}
		}

		public bool Moved
		{
			get
			{
				return _Moved;
			}
		}

		public bool MovedMoreThanOneTile
		{
			get
			{
				return _MovedMoreThanOneTile;
			}
		}

		public UnitStatus Status
		{
			get
			{
				return _Status;
			}
		}

		public Tile Position
		{
			get
			{
				return _Position;
			}
		}

		public Unit Passenger
		{
			get
			{
				return _Passenger;
			}
		}

		public Unit Carrier
		{
			get
			{
				return _Carrier;
			}
		}

		public Unit(Army Army, UnitConfiguration UnitConfiguration, IdGenerator IdGenerator)
		{
			this.Army = Army;
			_BaseConfiguration = UnitConfiguration;
			_Id = IdGenerator.GenerateId();
		}

		public OrderInvalidReason CanBeAttackedBy(Army Army)
		{
			if (Army.Configuration.Team == this.Army.Configuration.Team || Configuration.IsNeutral())
				return OrderInvalidReason.TARGET_TEAM;
			if (_Position == null) return OrderInvalidReason.ILLEGAL;
			if (Configuration.UnitClass == UnitClass.FORT)
			{
				if (Position.Units.Any(
					i => i != this && i.Army == this.Army && i.CanBeAttackedBy(Army) == OrderInvalidReason.NONE))
					return OrderInvalidReason.NONE;
			}
			if (!Army.CanSeeUnit(this)) return OrderInvalidReason.TARGET_CONCEALED;
			if (_Carrier != null) return OrderInvalidReason.UNIT_NO_ACTION;
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
			if (Tile.RulesCalculator.Water && !Configuration.CanCarryInWater && Passenger != null)
				return OrderInvalidReason.UNIT_NO_CARRY_IN_WATER;
			if (Terminal
				&& Tile.GetStackSize() + GetStackSize() > Army.Configuration.Faction.StackLimit
				&& !Tile.Units.Contains(this))
				return OrderInvalidReason.UNIT_STACK_LIMIT;
			return OrderInvalidReason.NONE;
		}

		public OrderInvalidReason CanMove(bool Combat)
		{
			if (_Position == null || Fired || _Status != UnitStatus.ACTIVE || _Carrier != null)
				return OrderInvalidReason.UNIT_NO_MOVE;
			if (RemainingMovement > 0)
			{
				if (Combat)
				{
					if (Configuration.CanOverrun) return OrderInvalidReason.NONE;
					if (Configuration.CanCloseAssault)
						return _MovedMoreThanOneTile ? OrderInvalidReason.UNIT_NO_MOVE : OrderInvalidReason.NONE;
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
			if (_Position == null || Fired || _MovedMoreThanOneTile || _Status != UnitStatus.ACTIVE || _Carrier != null)
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
			if (_Passenger != null) _Passenger.HandleCombatResult(CombatResult);
			switch (CombatResult)
			{
				case CombatResult.MISS:
					return;
				case CombatResult.DESTROY:
					_Status = UnitStatus.DESTROYED;
					if (OnDestroy != null) OnDestroy(this, EventArgs.Empty);
					Remove();
					return;
				case CombatResult.DISRUPT:
					_Status = UnitStatus.DISRUPTED;
					return;
				case CombatResult.DOUBLE_DISRUPT:
					if (_Status == UnitStatus.DISRUPTED)
					{
						_Status = UnitStatus.DESTROYED;
						if (OnDestroy != null) OnDestroy(this, EventArgs.Empty);
						Remove();
					}
					else _Status = UnitStatus.DISRUPTED;
					return;
			}
		}

		public void Remove()
		{
			_Position.Exit(this);
			_Position = null;
			if (OnRemove != null) OnRemove(this, EventArgs.Empty);
		}

		public void Place(Tile Tile, Path<Tile> Path = null)
		{
			if (_Passenger != null) _Passenger.Place(Tile, Path);
			if (_Position != null) _Position.Exit(this);
			_Position = Tile;
			_Position.Enter(this);

			if (OnMove != null) OnMove(this, new MovementEventArgs(Tile, Path));
		}

		public void MoveTo(Tile Tile, Path<Tile> Path)
		{
			if (Tile == _Position) return;
			foreach (Tile t in Path.Nodes) t.Control(this);

			float movement = (float)Path.Distance;
			_RemainingMovement -= movement;
			_MovedMoreThanOneTile = Path.Count > 2 || _Moved;
			_Moved = true;
			Place(Tile, Path);

		}

		public bool MustMove()
		{
			return Deployment.UnitMustMove(this);
		}

		public OrderInvalidReason CanDismount()
		{
			if (_Carrier != null || Status != UnitStatus.ACTIVE || Moved || Fired)
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
			if (_Passenger != null) return OrderInvalidReason.UNIT_CARRYING;
			if (Unit.Carrier != null) return OrderInvalidReason.TARGET_CARRIED;
			if (MustMove()) return OrderInvalidReason.UNIT_MUST_MOVE;
			if (Position != null && Position.RulesCalculator.Water && !Configuration.CanCarryInWater)
				return OrderInvalidReason.UNIT_NO_CARRY_IN_WATER;

			return Configuration.CanLoad(Unit.Configuration);
		}

		public OrderInvalidReason CanUnload()
		{
			if (Status != UnitStatus.ACTIVE) return OrderInvalidReason.UNIT_NO_MOVE;
			if (_Passenger == null) return OrderInvalidReason.UNIT_NO_PASSENGER;
			if (_Position != null)
			{
				OrderInvalidReason r = _Passenger.CanEnter(_Position);
				if (r != OrderInvalidReason.NONE) return r;

				if (_Position.GetStackSize() + _Passenger.Configuration.GetStackSize()
					> Army.Configuration.Faction.StackLimit)
					return OrderInvalidReason.UNIT_STACK_LIMIT;
			}
			if (MustMove()) return OrderInvalidReason.UNIT_MUST_MOVE;
			return OrderInvalidReason.NONE;
		}

		public bool IsSolitary()
		{
			return _Position.Units.Count() == 1 || _Position.Units.All(i => i == this || i == _Passenger);
		}

		public void Dismount(bool UseMovement)
		{
			_Dismounted = true;
			if (OnConfigurationChange != null) OnConfigurationChange(this, EventArgs.Empty);
			if (UseMovement)
			{
				_Moved = true;
				_RemainingMovement = 0;
			}
		}

		public void Mount(bool UseMovement)
		{
			_Dismounted = false;
			if (OnConfigurationChange != null) OnConfigurationChange(this, EventArgs.Empty);
			if (UseMovement)
			{
				_Moved = true;
				_RemainingMovement = 0;
			}
		}

		public void Load(Unit Unit, bool UseMovement)
		{
			_Passenger = Unit;
			Unit._Carrier = this;

			if (UseMovement)
			{
				_Moved = true;
				Unit._Moved = true;
				Unit._MovedMoreThanOneTile = true;
				_RemainingMovement = 0;
				Unit._RemainingMovement = 0;
			}

			if (OnLoad != null) OnLoad(this, EventArgs.Empty);
		}

		public void Unload(bool UseMovement)
		{
			if (UseMovement)
			{
				_Moved = true;
				_Passenger._Moved = true;
				_Passenger._MovedMoreThanOneTile = true;
				_RemainingMovement = 0;
				_Passenger._RemainingMovement = 0;
			}

			Unit passenger = _Passenger;
			_Passenger._Carrier = null;
			_Passenger = null;

			if (OnUnload != null) OnUnload(this, new ValuedEventArgs<Unit>(passenger));
		}

		public int GetStackSize()
		{
			return _Carrier == null ? Configuration.GetStackSize() : 0;
		}

		public LineOfSight GetLineOfSight(Tile Tile)
		{
			if (_Position == null) return null;
			return new LineOfSight(_Position, Tile);
		}

		public Path<Tile> GetPathTo(Tile Tile, bool Combat)
		{
			if (_Position == null) return null;
			return GetPathTo(_Position, Tile, Combat);
		}

		public Path<Tile> GetPathTo(Tile From, Tile Tile, bool Combat)
		{
			return new Path<Tile>(
				From,
				Tile,
				i => true,
				(i, j) => i.RulesCalculator.GetMoveCost(this, j, !Combat),
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
			if (_Position != null && CanAttack(AttackMethod) == OrderInvalidReason.NONE)
			{
				foreach (LineOfSight l in new Field<Tile>(_Position, Configuration.GetRange(AttackMethod), (i, j) => 1)
						 .GetReachableNodes()
						 .Select(i => GetLineOfSight(i.Item1))
						 .Where(i => i.Final != _Position))
				{
					if (l.Validate() == NoLineOfSightReason.NONE) yield return new Tuple<LineOfSight, bool>(l, true);
					else if (CanAttack(AttackMethod, false, l) == OrderInvalidReason.NONE)
						yield return new Tuple<LineOfSight, bool>(l, false);
				}
			}
		}

		public IEnumerable<Tuple<Tile, Tile, double>> GetFieldOfMovement(bool Combat)
		{
			if (_Position == null) return null;
			if (CanMove(Combat) != OrderInvalidReason.NONE) return Enumerable.Empty<Tuple<Tile, Tile, double>>();

			IEnumerable<Tuple<Tile, Tile, double>> adjacent =
				_Position.NeighborTiles
		   			.Where(i => i != null && _Position.RulesCalculator.CanMove(this, i, !Combat, false))
					.Select(i => new Tuple<Tile, Tile, double>(
							 i, _Position, _Position.RulesCalculator.GetMoveCost(this, i, !Combat)));
			if (Combat && Configuration.CanCloseAssault)
				return adjacent;

			IEnumerable<Tuple<Tile, Tile, double>> fullMovement = new Field<Tile>(
				_Position,
				RemainingMovement,
				(i, j) => i.RulesCalculator.GetMoveCost(this, j, !Combat))
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

		public void Fire()
		{
			_Fired = true;
			if (OnFire != null) OnFire(this, EventArgs.Empty);
		}

		public void Halt()
		{
			_Moved = true;
			_RemainingMovement = 0;
		}

		public void Reset()
		{
			_Fired = false;
			_Moved = false;
			_MovedMoreThanOneTile = false;
			_RemainingMovement = Configuration.GetMaxMovement(Army.Match.Scenario.Environment);
			if (_Status == UnitStatus.DISRUPTED) _Status = UnitStatus.ACTIVE;
		}

		public override string ToString()
		{
			return string.Format("[Unit: Id={0}, Configuration={1}]", Id, Configuration);
		}
	}
}
