using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileHasEdge : Matcher<Tile>
	{
		enum Attribute { EDGE }

		public readonly TileEdge Edge;

		public TileHasEdge(TileEdge Edge)
		{
			this.Edge = Edge;
		}

		public TileHasEdge(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			Edge = (TileEdge)attributes[(int)Attribute.EDGE];
		}

		public TileHasEdge(SerializationInputStream Stream)
			: this((TileEdge)Stream.ReadByte()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)Edge);
		}

		public bool Matches(Tile Tile)
		{
			return Tile.Configuration.Edges.Any(i => i == Edge);
		}

		public IEnumerable<Matcher<Tile>> Flatten()
		{
			yield return this;
		}
	}
}
