using System;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class FurthestAdvance : Objective
	{
		enum Attribute { DIRECTION }

		public readonly Direction Direction;

		int _Score;

		public FurthestAdvance(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			Direction = (Direction)attributes[(int)Attribute.DIRECTION];
		}

		public int CalculateScore(Army ForArmy, Match Match)
		{
			if (Direction == Direction.NORTH_WEST || Direction == Direction.NORTH_EAST || Direction == Direction.NORTH)
				_Score = Match.Map.Height - ForArmy.Units.Max(i => i.Position != null ? 0 : i.Position.Coordinate.Y);
			if (Direction == Direction.SOUTH_WEST || Direction == Direction.SOUTH_EAST || Direction == Direction.SOUTH)
				_Score = ForArmy.Units.Max(i => i.Position != null ? 0 : i.Position.Coordinate.Y) + 1;
			if (Direction == Direction.EAST)
				_Score = ForArmy.Units.Max(i => i.Position != null ? 0 : i.Position.Coordinate.X) + 1;
			if (Direction == Direction.WEST)
				_Score = Match.Map.Width - ForArmy.Units.Max(i => i.Position != null ? 0 : i.Position.Coordinate.Y);
			return _Score;
		}

		public int GetScore()
		{
			return _Score;
		}
	}
}
