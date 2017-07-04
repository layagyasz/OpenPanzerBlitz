using System;

using Cardamom.Graphing;

namespace PanzerBlitz
{
	public class OverrunMoveOrder : Order
	{
		Path<Tile> _InitialMovement;
		Tile _AttackTile;
		Tile _ExitTile;

		public OverrunMoveOrder(Path<Tile> InitialMovement, Tile AttackTile, Tile ExitTile)
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
