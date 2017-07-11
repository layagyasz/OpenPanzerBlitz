using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;

namespace PanzerBlitz
{
	public class Unit
	{
		public EventHandler<EventArgs> OnMove;
		public EventHandler<EventArgs> OnRemove;
		public EventHandler<EventArgs> OnDestroy;

		public readonly Army Army;
		public readonly UnitConfiguration UnitConfiguration;

		private bool _Deployed;

		private float _RemainingMovement;
		private bool _Fired;
		private bool _Moved;
		private bool _MovedMoreThanOneTile;
		private bool _Disrupted;
		private bool _Destroyed;

		private Tile _Position;

		public bool Deployed
		{
			get
			{
				return _Deployed;
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

		public Unit(Army Army, UnitConfiguration UnitConfiguration)
		{
			this.Army = Army;
			this.UnitConfiguration = UnitConfiguration;
		}

		public NoMoveReason CanMove(bool Combat)
		{
			if (Fired || Disrupted || Destroyed) return NoMoveReason.NO_MOVE;
			if (RemainingMovement > 0)
			{
				if (Combat)
				{
					if (UnitConfiguration.CanOverrun) return NoMoveReason.NONE;
					if (UnitConfiguration.CanCloseAssault)
						return _MovedMoreThanOneTile ? NoMoveReason.NO_MOVE : NoMoveReason.NONE;
					return NoMoveReason.NO_MOVE;
				}
				else return NoMoveReason.NONE;
			}
			else return NoMoveReason.NO_MOVE;
		}

		public NoMoveReason CanMove(bool Vehicle, bool Combat)
		{
			if (Vehicle != UnitConfiguration.IsVehicle) return NoMoveReason.NO_MOVE;
			else return CanMove(Combat);
		}

		public NoSingleAttackReason CanAttack(AttackMethod AttackMethod)
		{
			if (Fired || Disrupted || Destroyed) return NoSingleAttackReason.UNABLE;
			return UnitConfiguration.CanAttack(AttackMethod);
		}

		public NoSingleAttackReason CanAttack(AttackMethod AttackMethod, bool EnemyArmored, LineOfSight LineOfSight)
		{
			NoSingleAttackReason r = CanAttack(AttackMethod);
			if (r != NoSingleAttackReason.NONE) return r;
			return UnitConfiguration.CanAttack(AttackMethod, EnemyArmored, LineOfSight);
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
		}

		public void Remove()
		{
			_Position.Exit(this);
			_Position = null;
			if (OnRemove != null) OnRemove(this, EventArgs.Empty);
		}

		public void Place(Tile Tile)
		{
			if (_Position != null) _Position.Exit(this);
			_Position = Tile;
			_Position.Enter(this);
			if (OnMove != null) OnMove(this, new MovementEventArgs());
		}

		public void MoveTo(Tile Tile, float Movement)
		{
			_RemainingMovement -= Movement;
			_MovedMoreThanOneTile = Movement > 1 || _Moved;
			_Moved = true;
			Place(Tile);
		}

		public void Deploy()
		{
			_Deployed = true;
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
				(i, j) => i.MovementProfile.GetMoveCost(this, j, !Combat),
				(i, j) => i.HeuristicDistanceTo(j),
				i => i.Neighbors(),
				(i, j) => i == j);
		}

		public IEnumerable<LineOfSight> GetFieldOfSight(AttackMethod AttackMethod)
		{
			if (CanAttack(AttackMethod) != NoSingleAttackReason.NONE) return Enumerable.Empty<LineOfSight>();

			return new Field<Tile>(_Position, UnitConfiguration.GetRange(AttackMethod), (i, j) => 1)
				.GetReachableNodes()
				.Select(i => GetLineOfSight(i.Item1))
				.Where(i => i.Final != _Position && i.Validate() == NoLineOfSightReason.NONE);
		}

		public IEnumerable<Tuple<Tile, Tile, double>> GetFieldOfMovement(bool Combat)
		{
			if (CanMove(Combat) != NoMoveReason.NONE) return Enumerable.Empty<Tuple<Tile, Tile, double>>();

			IEnumerable<Tuple<Tile, Tile, double>> adjacent =
				_Position.NeighborTiles
		   			.Where(i => i != null && _Position.MovementProfile.CanMove(this, i, !Combat, false))
					.Select(i => new Tuple<Tile, Tile, double>(
							 i, _Position, _Position.MovementProfile.GetMoveCost(this, i, !Combat)));
			if (Combat && UnitConfiguration.CanCloseAssault)
				return adjacent;

			IEnumerable<Tuple<Tile, Tile, double>> fullMovement = new Field<Tile>(
				_Position,
				RemainingMovement,
				(i, j) => i.MovementProfile.GetMoveCost(this, j, !Combat))
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
			_RemainingMovement = UnitConfiguration.Movement;
			_Disrupted = false;
		}
	}
}
