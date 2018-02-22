using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileElevation : Matcher<Tile>
	{
		enum Attribute { ELEVATION, ATLEAST }

		public readonly int Elevation;
		public readonly bool Atleast;

		public TileElevation(int Elevation, bool Atleast)
		{
			this.Elevation = Elevation;
			this.Atleast = Atleast;
		}

		public TileElevation(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Elevation = (int)attributes[(int)Attribute.ELEVATION];
			Atleast = Parse.DefaultIfNull(attributes[(int)Attribute.ATLEAST], false);
		}

		public TileElevation(SerializationInputStream Stream)
			: this(Stream.ReadInt32(), Stream.ReadBoolean()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Elevation);
			Stream.Write(Atleast);
		}

		public bool Matches(Tile Tile)
		{
			if (Tile == null) return false;
			return Tile.Configuration.Elevation < Elevation ^ Atleast;
		}

		public IEnumerable<Matcher<Tile>> Flatten()
		{
			yield return this;
		}
	}
}
