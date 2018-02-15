using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class FurthestAdvanceObjective : Objective
	{
		enum Attribute { DIRECTION }

		public readonly Direction Direction;

		public FurthestAdvanceObjective(Direction Direction)
		{
			this.Direction = Direction;
		}

		public FurthestAdvanceObjective(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			Direction = (Direction)attributes[(int)Attribute.DIRECTION];
		}

		public FurthestAdvanceObjective(SerializationInputStream Stream)
			: this((Direction)Stream.ReadByte()) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)Direction);
		}

		public override bool CanStopEarly()
		{
			return false;
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			switch (Direction)
			{
				case Direction.NORTH_WEST:
				case Direction.NORTH_EAST:
				case Direction.NORTH:
					return Match.Map.Height - ForArmy.Units.Max(i => i.Position != null ? 0 : i.Position.Coordinate.Y);
				case Direction.SOUTH_WEST:
				case Direction.SOUTH_EAST:
				case Direction.SOUTH:
					return ForArmy.Units.Max(i => i.Position != null ? 0 : i.Position.Coordinate.Y) + 1;
				case Direction.EAST:
					return ForArmy.Units.Max(i => i.Position != null ? 0 : i.Position.Coordinate.X) + 1;
				case Direction.WEST:
					return Match.Map.Width - ForArmy.Units.Max(i => i.Position != null ? 0 : i.Position.Coordinate.Y);
				default:
					throw new ArgumentException(string.Format("Direction not supported: {0}", Direction));
			}
		}

		public override IEnumerable<Tile> GetTiles(Map Map)
		{
			return Enumerable.Empty<Tile>();
		}
	}
}
