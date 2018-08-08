using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileHasEdge : Matcher<Tile>
	{
		enum Attribute { EDGE }

		public readonly TileEdge Edge;

		public override bool IsTransient { get; } = false;

		public TileHasEdge(TileEdge Edge)
		{
			this.Edge = Edge;
		}

		public TileHasEdge(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Edge = (TileEdge)attributes[(int)Attribute.EDGE];
		}

		public TileHasEdge(SerializationInputStream Stream)
			: this((TileEdge)Stream.ReadByte()) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)Edge);
		}

		public override bool Matches(Tile Object)
		{
			if (Object == null) return false;
			return Object.Configuration.Edges.Any(i => i == Edge);
		}
	}
}
