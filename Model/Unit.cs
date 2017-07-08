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
		private bool _Disrupted;

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

		public bool Disrupted
		{
			get
			{
				return _Disrupted;
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
			if (Fired || Disrupted) return NoMoveReason.NO_MOVE;
			if (RemainingMovement > 0)
			{
				if (Combat)
					return UnitConfiguration.CanOverrun || UnitConfiguration.CanCloseAssault
											? NoMoveReason.NONE : NoMoveReason.NO_MOVE;
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
			if (Fired || Disrupted) return NoSingleAttackReason.UNABLE;
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
					Remove();
					if (OnDestroy != null) OnDestroy(this, EventArgs.Empty);
					return;
				case CombatResult.DISRUPT:
					_Disrupted = true;
					return;
				case CombatResult.DOUBLE_DISRUPT:
					if (_Disrupted)
					{
						Remove();
						if (OnDestroy != null) OnDestroy(this, EventArgs.Empty);
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
			return new Field<Tile>(_Position, UnitConfiguration.GetRange(AttackMethod), (i, j) => 1)
				.GetReachableNodes()
				.Select(i => GetLineOfSight(i.Item1))
				.Where(i => i.Final != _Position && i.Verify() == NoLineOfSightReason.NONE);
		}

		public IEnumerable<Tuple<Tile, Tile, double>> GetFieldOfMovement(bool Combat)
		{
			return new Field<Tile>(
				_Position,
				RemainingMovement,
				(i, j) => i.MovementProfile.GetMoveCost(this, j, !Combat)).GetReachableNodes();
		}

		public void Fire()
		{
			_Fired = true;
		}

		public void Reset()
		{
			_Fired = false;
			_Moved = false;
			_RemainingMovement = UnitConfiguration.Movement;
			_Disrupted = false;
		}
	}
}
