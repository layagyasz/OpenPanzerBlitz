using System;

namespace PanzerBlitz
{
	public class OverrunMoveOrder : Order
	{
		MovementOrder _InitialMovement;
		Tile _AttackTile;
		Tile _ExitTile;

		public OverrunMoveOrder(MovementOrder InitialMovement, Tile AttackTile)
		{
			_InitialMovement = InitialMovement;
			_AttackTile = AttackTile;
			_ExitTile = AttackTile.GetOppositeNeighbor(InitialMovement.Path.Destination);
		}

		public NoMoveReason Validate()
		{
			return NoMoveReason.NONE;
		}

		public bool Execute(Random Random)
		{
			return _InitialMovement.Execute(Random);
		}
	}
}
