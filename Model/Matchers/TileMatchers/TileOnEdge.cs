using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileOnEdge : Matcher<Tile>
	{
		enum Attribute { EDGE }

		public readonly Direction Edge;

		public override bool IsTransient { get; } = false;

		public TileOnEdge(Direction Edge)
		{
			this.Edge = Edge;
		}

		public TileOnEdge(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Edge = (Direction)attributes[(int)Attribute.EDGE];
		}

		public TileOnEdge(SerializationInputStream Stream)
			: this((Direction)Stream.ReadByte()) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)Edge);
		}

		public override bool Matches(Tile Object)
		{
			if (Object == null) return false;
			return Object.OnEdge(Edge);
		}
	}
}
