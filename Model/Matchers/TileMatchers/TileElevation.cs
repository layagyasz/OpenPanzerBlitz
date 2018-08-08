using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileElevation : Matcher<Tile>
	{
		enum Attribute { ELEVATION, ATLEAST }

		public readonly int Elevation;
		public readonly bool Atleast;

		public override bool IsTransient { get; } = false;

		public TileElevation(int Elevation, bool Atleast)
		{
			this.Elevation = Elevation;
			this.Atleast = Atleast;
		}

		public TileElevation(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Elevation = (int)attributes[(int)Attribute.ELEVATION];
			Atleast = (bool)(attributes[(int)Attribute.ATLEAST] ?? false);
		}

		public TileElevation(SerializationInputStream Stream)
			: this(Stream.ReadInt32(), Stream.ReadBoolean()) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Elevation);
			Stream.Write(Atleast);
		}

		public override bool Matches(Tile Object)
		{
			if (Object == null) return false;
			return Object.Configuration.Elevation < Elevation ^ Atleast;
		}
	}
}
