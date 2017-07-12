using System;
using System.Linq;

namespace PanzerBlitz
{
	public class OverrunMoveOrder : Order
	{
		MovementOrder _InitialMovement;
		Tile _AttackTile;
		Tile _ExitTile;
		float _Distance;
		NoMoveReason _Validate;

		public OverrunMoveOrder(MovementOrder InitialMovement, Tile AttackTile)
		{
			_InitialMovement = InitialMovement;
			_AttackTile = AttackTile;
			_ExitTile = AttackTile.GetOppositeNeighbor(InitialMovement.Path.Destination);

			_Validate = InitialValidate();
		}

		private NoMoveReason InitialValidate()
		{
			NoMoveReason r = _InitialMovement.Validate();
			if (r != NoMoveReason.NONE) return r;
			if (_ExitTile.IsEnemyOccupied(_InitialMovement.Unit.Army)) return NoMoveReason.ENEMY_OCCUPIED;
			if (_ExitTile.Units.Count() >= _InitialMovement.Unit.Army.ArmyConfiguration.Faction.StackLimit)
				return NoMoveReason.STACK_LIMIT;

			float distance1 = _InitialMovement.Path.Destination.MovementProfile.GetMoveCost(
				_InitialMovement.Unit, _AttackTile, false, true);
			float distance2 = _AttackTile.MovementProfile.GetMoveCost(_InitialMovement.Unit, _ExitTile, false, false);
			if (Math.Abs(distance1 - float.MaxValue) < float.Epsilon
				|| Math.Abs(distance2 - float.MaxValue) < float.Epsilon) return NoMoveReason.TERRAIN;

			_Distance = (float)_InitialMovement.Path.Distance + distance1 + distance2;
			if (_Distance > _InitialMovement.Unit.RemainingMovement) return NoMoveReason.NO_MOVE;
			return NoMoveReason.NONE;
		}

		public NoMoveReason Validate()
		{
			return _Validate;
		}

		public bool Execute(Random Random)
		{
			if (Validate() == NoMoveReason.NONE)
			{
				_InitialMovement.Unit.MoveTo(_ExitTile, _Distance);
				return true;
			}
			else return false;
		}
	}
}
