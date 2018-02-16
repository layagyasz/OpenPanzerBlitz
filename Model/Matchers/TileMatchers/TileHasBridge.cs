using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileHasBridge : Matcher<Tile>
	{
		public TileHasBridge(ParseBlock Block) { }

		public TileHasBridge(SerializationInputStream Stream) { }

		public void Serialize(SerializationOutputStream Stream) { }

		public bool Matches(Tile Tile)
		{
			if (Tile == null) return false;
			return Tile.Rules.Bridged;
		}

		public IEnumerable<Matcher<Tile>> Flatten()
		{
			yield return this;
		}
	}
}
