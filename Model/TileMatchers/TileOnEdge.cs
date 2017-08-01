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
			return Tile.OnEdge(Edge);
		}
	}
}
