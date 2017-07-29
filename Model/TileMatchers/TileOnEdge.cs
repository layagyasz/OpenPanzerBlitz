using System;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileOnEdge : Matcher
	{
		enum Attribute { EDGE }

		public readonly Direction Edge;

		public TileOnEdge(Direction Edge)
		{
			this.Edge = Edge;
		}

		public TileOnEdge(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			Edge = (Direction)attributes[(int)Attribute.EDGE];
		}

		public bool Matches(Tile Tile)
		{
			if (Edge == Direction.NONE) return true;
			if (Edge == Direction.ANY) return Tile.NeighborTiles.Any(i => i == null);
			if (Edge == Direction.NORTH)
				return Tile.NeighborTiles[(int)Direction.NORTH_WEST] == null
						   && Tile.NeighborTiles[(int)Direction.NORTH_EAST] == null;
			if (Edge == Direction.SOUTH)
				return Tile.NeighborTiles[(int)Direction.SOUTH_WEST] == null
						   && Tile.NeighborTiles[(int)Direction.SOUTH_EAST] == null;
			return Tile.NeighborTiles[(int)Edge] == null;
		}
	}
}
