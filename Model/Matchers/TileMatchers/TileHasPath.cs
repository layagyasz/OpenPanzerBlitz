using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileHasPath : Matcher<Tile>
	{
		enum Attribute { PATH }

		public readonly TilePathOverlay Path;

		public override bool IsTransient { get; } = false;

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

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)Path);
		}

		public override bool Matches(Tile Object)
		{
			if (Object == null) return false;
			return Object.Configuration.PathOverlays.Any(i => i == Path);
		}
	}
}
