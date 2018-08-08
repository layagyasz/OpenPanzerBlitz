using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileHasBase : Matcher<Tile>
	{
		enum Attribute { BASE };

		public readonly TileBase TileBase;

		public override bool IsTransient { get; } = false;

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

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)TileBase);
		}

		public override bool Matches(Tile Object)
		{
			if (Object == null) return false;
			return Object.Configuration.TileBase == TileBase;
		}
	}
}
