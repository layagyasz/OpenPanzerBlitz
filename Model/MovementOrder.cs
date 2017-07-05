using System;

using Cardamom.Graphing;

namespace PanzerBlitz
{
	public class MovementOrder : Order
	{
		Path<Tile> _Path;

		public MovementOrder(Unit Unit, bool Combat, Tile From, Tile To)
		{
			_Path = new Path<Tile>(
				From,
				To,
				i => true,
				(i, j) => i.MovementProfile.GetMoveCost(Unit, j, !Combat),
				(i, j) => i.HeuristicDistanceTo(j),
				i => i.Neighbors(),
				(i, j) => i == j);
		}

		public bool Execute(Random Random)
		{
			return true;
		}
	}
}
