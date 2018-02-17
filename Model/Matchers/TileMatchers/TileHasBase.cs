using System.Collections.Generic;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileHasBase : Matcher<Tile>
	{
		enum Attribute { BASE };

		public readonly TileBase TileBase;

		public TileHasBase(TileBase TileBase)
		{
			this.TileBase = TileBase;
		}

		public TileHasBase(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			TileBase = (TileBase)attributes[(int)Attribute.BASE];
		}

		public TileHasBase(SerializationInputStream Stream)
			: this((TileBase)Stream.ReadByte()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)TileBase);
		}

		public bool Matches(Tile Tile)
		{
			return Tile.Configuration.TileBase == TileBase;
		}

		public IEnumerable<Matcher<Tile>> Flatten()
		{
			yield return this;
		}
	}
}
