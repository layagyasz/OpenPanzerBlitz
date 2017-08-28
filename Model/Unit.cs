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
		public EventHandler<MovementEventArgs> OnMove;
		public EventHandler<EventArgs> OnFire;
		public EventHandler<EventArgs> OnRemove;
		public EventHandler<EventArgs> OnDestroy;

		public readonly Army Army;
		public readonly UnitConfiguration Configuration;

		int _Id;

		private bool _Deployed;

		private float _RemainingMovement;
		private bool _Fired;
		private bool _Moved;
		private bool _MovedMoreThanOneTile;
		private UnitStatus _Status;

		private Tile _Position;

		private Unit _Passenger;
		private Unit _Carrier;

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
			this.Configuration = UnitConfiguration;
			_Id = IdGenerator.GenerateId();
		}

		public NoSingleAttackReason CanBeAttackedBy(Army Army)
		{
			if (Army.Configuration.Team == this.Army.Configuration.Team || Configuration.IsNeutral())
				return NoSingleAttackReason.TEAM;
			if (_Carrier != null) return NoSingleAttackReason.PASSENGER;
			if (_Position == null) return NoSingleAttackReason.ILLEGAL;
			if (_Position.Configuration.Concealing && !Army.CanSeeUnit(this)) return NoSingleAttackReason.CONCEALED;
			return NoSingleAttackReason.NONE;
		}

		public NoDeployReason CanEnter(Tile Tile, bool Terminal = false)
		{
			if (Tile.GetBlockType() == BlockType.STANDARD
				&& Tile.Units.Any(i => !i.Configuration.IsNeutral() && i.Army != Army))
				return NoDeployReason.ENEMY_OCCUPIED;
			if (Configuration.IsStackUnique() && Tile.Units.Any(i => i != this && i.Configuration.IsStackUnique()))
				return NoDeployReason.UNIQUE;
			if (Terminal && Tile.GetStackSize() + GetStackSize() > Army.Configuration.Faction.StackLimit)
				return NoDeployReason.STACK_LIMIT;
			return NoDeployReason.NONE;
		}

		public NoMoveReason CanMove(bool Combat)
		{
			if (_Position == null || Fired || _Status != UnitStatus.ACTIVE || _Carrier != null)
				return NoMoveReason.NO_MOVE;
			if (RemainingMovement > 0)
			{
				if (Combat)
				{
					if (Configuration.CanOverrun) return NoMoveReason.NONE;
					if (Configuration.CanCloseAssault)
						return _MovedMoreThanOneTile ? NoMoveReason.NO_MOVE : NoMoveReason.NONE;
					return NoMoveReason.NO_MOVE;
				}
				return NoMoveReason.NONE;
			}
			return NoMoveReason.NO_MOVE;
		}

		public NoMoveReason CanMove(bool Vehicle, bool Combat)
		{
			if (Vehicle != Configuration.IsVehicle) return NoMoveReason.NO_MOVE;
			return CanMove(Combat);
		}

		public NoSingleAttackReason CanAttack(AttackMethod AttackMethod)
		{
			if (_Position == null || Fired || _Status != UnitStatus.ACTIVE || _Carrier != null)
				return NoSingleAttackReason.UNABLE;
			if (MustMove()) return NoSingleAttackReason.MUST_MOVE;
			return Configuration.CanAttack(AttackMethod);
		}

		public NoSingleAttackReason CanAttack(AttackMethod AttackMethod, bool EnemyArmored, LineOfSight LineOfSight)
		{
			NoSingleAttackReason r = CanAttack(AttackMethod);
			if (r != NoSingleAttackReason.NONE) return r;

			if (AttackMethod == AttackMethod.NORMAL_FIRE && LineOfSight.Validate() != NoLineOfSightReason.NONE)
				return NoSingleAttackReason.NO_LOS;

			if (AttackMethod == AttackMethod.NORMAL_FIRE)
			{
				if (Configuration.CanDirectFireAt(EnemyArmored, LineOfSight) == NoSingleAttackReason.NONE)
					return NoSingleAttackReason.NONE;
				r = Configuration.CanIndirectFireAt(LineOfSight);
				if (r != NoSingleAttackReason.NONE) return r;
				if (!Army.CanIndirectFireAtTile(LineOfSight.Final))
					return NoSingleAttackReason.NO_INDIRECT_FIRE_SPOTTER;
				return NoSingleAttackReason.NONE;
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
			float movement = (float)Path.Distance;
			_RemainingMovement -= movement;
			_MovedMoreThanOneTile = movement > 1 || _Moved;
			_Moved = true;
			Place(Tile, Path);
		}

		public bool MustMove()
		{
			if (Deployment is ConvoyDeployment)
				return _Position != null
					&& ((ConvoyDeployment)Deployment).EntryTile == _Position
					 && ((ConvoyDeployment)Deployment).ConvoyOrder.Count() > 0;
			return false;
		}

		public NoLoadReason CanLoad(Unit Unit)
		{
			if (Unit.Moved || Moved || Unit.Fired) return NoLoadReason.NO_MOVE;
			if (Unit.Army != Army) return NoLoadReason.TEAM;
			if (Unit.Position != Position) return NoLoadReason.ILLEGAL;
			if (_Passenger != null) return NoLoadReason.CARRYING;
			if (Unit.Carrier != null) return NoLoadReason.CARRIED;
			if (MustMove()) return NoLoadReason.MUST_MOVE;

			return Configuration.CanLoad(Unit.Configuration);
		}

		public NoUnloadReason CanUnload()
		{
			if (_Passenger == null) return NoUnloadReason.NO_PASSENGER;
			if (_Position != null)
			{
				NoDeployReason r = _Passenger.CanEnter(_Position);
				if (r != NoDeployReason.NONE) return EnumConverter.ConvertToNoUnloadReason(r);
			}
			if (MustMove()) return NoUnloadReason.MUST_MOVE;
			return NoUnloadReason.NONE;
		}

		public bool IsSolitary()
		{
			return _Position.Units.Count() == 1 || _Position.Units.All(i => i == this || i == _Passenger);
		}

		public void Load(Unit Unit, bool UseMovement)
		{
			_Passenger = Unit;
			Unit._Carrier = this;

			if (UseMovement)
			{
				_Moved = true;
				Unit._Moved = true;
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
			return new Path<Tile>(
				_Position,
				Tile,
				i => true,
				(i, j) => i.Configuration.GetMoveCost(this, j, !Combat),
				(i, j) => i.HeuristicDistanceTo(j),
				i => i.Neighbors(),
				(i, j) => i == j);
		}

		public IEnumerable<Tile> GetFieldOfDeployment(IEnumerable<Tile> Tiles)
		{
			if (Deployment is PositionalDeployment)
				return Tiles.Where(i => ((PositionalDeployment)Deployment).Validate(this, i) == NoDeployReason.NONE);
			return Enumerable.Empty<Tile>();
		}

		public IEnumerable<Tuple<LineOfSight, bool>> GetFieldOfSight(AttackMethod AttackMethod)
		{
			if (_Position != null && CanAttack(AttackMethod) == NoSingleAttackReason.NONE)
			{
				foreach (LineOfSight l in new Field<Tile>(_Position, Configuration.GetRange(AttackMethod), (i, j) => 1)
						 .GetReachableNodes()
						 .Select(i => GetLineOfSight(i.Item1))
						  .Where(i => i.Final != _Position))
				{
					if (l.Validate() == NoLineOfSightReason.NONE) yield return new Tuple<LineOfSight, bool>(l, true);
					else if (CanAttack(AttackMethod, false, l) == NoSingleAttackReason.NONE)
						yield return new Tuple<LineOfSight, bool>(l, false);
				}
			}
		}

		public IEnumerable<Tuple<Tile, Tile, double>> GetFieldOfMovement(bool Combat)
		{
			if (_Position == null) return null;
			if (CanMove(Combat) != NoMoveReason.NONE) return Enumerable.Empty<Tuple<Tile, Tile, double>>();

			IEnumerable<Tuple<Tile, Tile, double>> adjacent =
				_Position.NeighborTiles
		   			.Where(i => i != null && _Position.Configuration.CanMove(this, i, !Combat, false))
					.Select(i => new Tuple<Tile, Tile, double>(
							 i, _Position, _Position.Configuration.GetMoveCost(this, i, !Combat)));
			if (Combat && Configuration.CanCloseAssault)
				return adjacent;

			IEnumerable<Tuple<Tile, Tile, double>> fullMovement = new Field<Tile>(
				_Position,
				RemainingMovement,
				(i, j) => i.Configuration.GetMoveCost(this, j, !Combat))
					.GetReachableNodes();

			if (!Moved)
				return fullMovement.Concat(adjacent.Where(i => !fullMovement.Any(j => i.Item1 == j.Item1)));
			return fullMovement;
		}

		public void Fire()
		{
			_Fired = true;
			if (OnFire != null) OnFire(this, EventArgs.Empty);
		}

		public void Reset()
		{
			_Fired = false;
			_Moved = false;
			_MovedMoreThanOneTile = false;
			_RemainingMovement = Configuration.Movement;
			if (_Status == UnitStatus.DISRUPTED) _Status = UnitStatus.ACTIVE;
		}
	}
}
