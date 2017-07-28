using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileElevation : Matcher
	{
		enum Attribute { ELEVATION, ATLEAST }

		public readonly int Elevation;
		public readonly bool Atleast;

		public TileElevation(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Elevation = (int)attributes[(int)Attribute.ELEVATION];
			Atleast = Parse.DefaultIfNull(attributes[(int)Attribute.ATLEAST], false);
		}

		public bool Matches(Tile Tile)
		{
			return Tile.Elevation < Elevation ^ Atleast;
		}
	}
}
