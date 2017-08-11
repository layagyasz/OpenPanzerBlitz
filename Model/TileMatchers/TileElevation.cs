﻿using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileElevation : Matcher
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
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

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
			return Tile.Elevation < Elevation ^ Atleast;
		}
	}
}