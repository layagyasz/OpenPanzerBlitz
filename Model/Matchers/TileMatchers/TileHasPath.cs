using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileHasPath : Matcher<Tile>
	{
		enum Attribute { PATH }

		public readonly TilePathOverlay Path;

		public TileHasPath(TilePathOverlay Path)
		{
			this.Path = Path;
		}

		public TileHasPath(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Path = (TilePathOverlay)attributes[(int)Attribute.PATH];
		}

		public TileHasPath(SerializationInputStream Stream)
			: this((TilePathOverlay)Stream.ReadByte()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)Path);
		}

		public bool Matches(Tile Tile)
		{
			return Tile.Configuration.PathOverlays.Any(i => i == Path);
		}

		public IEnumerable<Matcher<Tile>> Flatten()
		{
			yield return this;
		}
	}
}
