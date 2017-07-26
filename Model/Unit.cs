using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;

namespace PanzerBlitz
{
	public class Unit
	{
		public EventHandler<EventArgs> OnLoad;
		public EventHandler<EventArgs> OnUnload;
		public EventHandler<MovementEventArgs> OnMove;
		public EventHandler<EventArgs> OnRemove;
		public EventHandler<EventArgs> OnDestroy;

		public readonly Army Army;
		public readonly UnitConfiguration Configuration;

		private bool _Deployed;

		private float _RemainingMovement;
		private bool _Fired;
		private bool _Moved;
		private bool _MovedMoreThanOneTile;
		private bool _Disrupted;
		private bool _Destroyed;

		private Tile _Position;

		private Unit _Passenger;
		private Unit _Carrier;

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

		public bool Disrupted
		{
			get
			{
				return _Disrupted;
			}
		}

		public bool Destroyed
		{
			get
			{
				return _Destroyed;
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

		public Unit(Army Army, UnitConfiguration UnitConfiguration)
		{
			this.Army = Army;
			this.Configuration = UnitConfiguration;
		}

		public NoDeployReason CanEnter(Tile Tile, bool Terminal = false)
		{
			if (Tile.GetBlockType() == BlockType.STANDARD && Tile.Units.Any(i => i.Army != Army))
				return NoDeployReason.ENEMY_OCCUPIED;
			if (Configuration.IsStackUnique() && Tile.Units.Any(i => i != this && i.Configuration.IsStackUnique()))
				return NoDeployReason.UNIQUE;
			if (Terminal && Tile.GetStackSize() + GetStackSize() > Army.Configuration.Faction.StackLimit)
				return NoDeployReason.STACK_LIMIT;
			return NoDeployReason.NONE;
		}

		public NoMoveReason CanMove(bool Combat)
		{
			if (_Position == null || Fired || Disrupted || Destroyed || _Carrier != null) return NoMoveReason.NO_MOVE;
			if (RemainingMovement > 0)
			{
				if (Combat)
				{
					if (Configuration.CanOverrun) return NoMoveReason.NONE;
					if (Configuration.CanCloseAssault)
						return _MovedMoreThanOneTile ? NoMoveReason.NO_MOVE : NoMoveReason.NONE;
					return NoMoveReason.NO_MOVE;
				}
				else return NoMoveReason.NONE;
			}
			else return NoMoveReason.NO_MOVE;
		}

		public NoMoveReason CanMove(bool Vehicle, bool Combat)
		{
			if (Vehicle != Configuration.IsVehicle) return NoMoveReason.NO_MOVE;
			else return CanMove(Combat);
		}

		public NoSingleAttackReason CanAttack(AttackMethod AttackMethod)
		{
			if (_Position == null || Fired || Disrupted || Destroyed || _Carrier != null)
				return NoSingleAttackReason.UNABLE;
			if (MustMove()) return NoSingleAttackReason.MUST_MOVE;
			return Configuration.CanAttack(AttackMethod);
		}

		public NoSingleAttackReason CanAttack(AttackMethod AttackMethod, bool EnemyArmored, LineOfSight LineOfSight)
		{
			NoSingleAttackReason r = CanAttack(AttackMethod);
			if (r != NoSingleAttackReason.NONE) return r;
			return Configuration.CanAttack(AttackMethod, EnemyArmored, LineOfSight);
		}

		public void HandleCombatResult(CombatResult CombatResult)
		{
			switch (CombatResult)
			{
				case CombatResult.MISS:
					return;
				case CombatResult.DESTROY:
					_Destroyed = true;
					if (OnDestroy != null) OnDestroy(this, EventArgs.Empty);
					Remove();
					return;
				case CombatResult.DISRUPT:
					_Disrupted = true;
					return;
				case CombatResult.DOUBLE_DISRUPT:
					if (_Disrupted)
					{
						_Destroyed = true;
						if (OnDestroy != null) OnDestroy(this, EventArgs.Empty);
						Remove();
					}
					else _Disrupted = true;
					return;
			}
			if (_Passenger != null) _Passenger.HandleCombatResult(CombatResult);
		}

		public void Remove()
		{
			_Position.Exit(this);
			_Position = null;
			if (OnRemove != null) OnRemove(this, EventArgs.Empty);
		}

		public void Place(Tile Tile)
		{
			if (_Passenger != null) _Passenger.Place(Tile);
			if (_Position != null) _Position.Exit(this);
			_Position = Tile;
			_Position.Enter(this);

			if (OnMove != null) OnMove(this, new MovementEventArgs(Tile));
		}

		public void MoveTo(Tile Tile, float Movement)
		{
			_RemainingMovement -= Movement;
			_MovedMoreThanOneTile = Movement > 1 || _Moved;
			_Moved = true;
			Place(Tile);
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
			if (Unit.Moved || Moved || Unit.Fired || Fired) return NoLoadReason.NO_MOVE;
			if (Unit.Army != Army) return NoLoadReason.TEAM;
			if (Unit.Position != Position) return NoLoadReason.ILLEGAL;
			if (_Passenger != null) return NoLoadReason.CARRYING;
			if (Unit.Carrier != null) return NoLoadReason.CARRIED;
			if (MustMove()) return NoLoadReason.MUST_MOVE;

			return Configuration.CanLoad(Unit.Configuration);
		}

		public NoUnloadReason CanUnload()
		{
			if (Fired) return NoUnloadReason.NO_MOVE;
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

			_Passenger._Carrier = null;
			_Passenger = null;

			if (OnUnload != null) OnUnload(this, EventArgs.Empty);
		}

		public int GetStackSize()
		{
			return _Carrier == null ? Configuration.GetStackSize() : 0;
		}

		public LineOfSight GetLineOfSight(Tile Tile)
		{
			return new LineOfSight(_Position, Tile);
		}

		public Path<Tile> GetPathTo(Tile Tile, bool Combat)
		{
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

		public IEnumerable<LineOfSight> GetFieldOfSight(AttackMethod AttackMethod)
		{
			if (CanAttack(AttackMethod) != NoSingleAttackReason.NONE) return Enumerable.Empty<LineOfSight>();

			return new Field<Tile>(_Position, Configuration.GetRange(AttackMethod), (i, j) => 1)
				.GetReachableNodes()
				.Select(i => GetLineOfSight(i.Item1))
				.Where(i => i.Final != _Position && i.Validate() == NoLineOfSightReason.NONE);
		}

		public IEnumerable<Tuple<Tile, Tile, double>> GetFieldOfMovement(bool Combat)
		{
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
		}

		public void Reset()
		{
			_Fired = false;
			_Moved = false;
			_MovedMoreThanOneTile = false;
			_RemainingMovement = Configuration.Movement;
			_Disrupted = false;
		}
	}
}
