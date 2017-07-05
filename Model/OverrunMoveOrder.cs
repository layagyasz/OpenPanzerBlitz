using System;

namespace PanzerBlitz
{
	public class OverrunMoveOrder : Order
	{
		MovementOrder _InitialMovement;
		Tile _AttackTile;
		Tile _ExitTile;

		public OverrunMoveOrder(MovementOrder InitialMovement, Tile AttackTile, Tile ExitTile)
		{
			_InitialMovement = InitialMovement;
			_AttackTile = AttackTile;
			_ExitTile = ExitTile;
		}

		public bool Execute(Random Random)
		{
			return true;
		}
	}
}
