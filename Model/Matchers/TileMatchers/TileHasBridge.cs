using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileHasBridge : Matcher<Tile>
	{
		public override bool IsTransient { get; } = false;

		public TileHasBridge(ParseBlock Block) { }

		public TileHasBridge(SerializationInputStream Stream) { }

		public override void Serialize(SerializationOutputStream Stream) { }

		public override bool Matches(Tile Object)
		{
			if (Object == null) return false;
			return Object.Rules.Bridged;
		}
	}
}
