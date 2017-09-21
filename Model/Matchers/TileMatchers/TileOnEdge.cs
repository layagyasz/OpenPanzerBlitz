using System;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileOnEdge : Matcher<Tile>
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

		public TileOnEdge(SerializationInputStream Stream)
			: this((Direction)Stream.ReadByte()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)Edge);
		}

		public bool Matches(Tile Tile)
		{
			if (Tile == null) return false;
			return Tile.OnEdge(Edge);
		}
	}
}
