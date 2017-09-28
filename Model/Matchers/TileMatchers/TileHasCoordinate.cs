using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileHasCoordinate : Matcher<Tile>
	{
		enum Attribute { COORDINATE }

		public readonly Coordinate Coordinate;

		public TileHasCoordinate(Coordinate Coordinate)
		{
			this.Coordinate = Coordinate;
		}

		public TileHasCoordinate(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Coordinate = (Coordinate)attributes[(int)Attribute.COORDINATE];
		}

		public TileHasCoordinate(SerializationInputStream Stream)
			: this(new Coordinate(Stream)) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Coordinate);
		}

		public bool Matches(Tile Tile)
		{
			return Tile.Coordinate == Coordinate;
		}
	}
}
