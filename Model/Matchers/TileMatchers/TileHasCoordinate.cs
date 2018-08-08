using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileHasCoordinate : Matcher<Tile>
	{
		enum Attribute { COORDINATE }

		public readonly Coordinate Coordinate;

		public override bool IsTransient { get; } = false;

		public TileHasCoordinate(Coordinate Coordinate)
		{
			this.Coordinate = Coordinate;
		}

		public TileHasCoordinate(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Coordinate = (Coordinate)attributes[(int)Attribute.COORDINATE];
		}

		public TileHasCoordinate(SerializationInputStream Stream)
			: this(new Coordinate(Stream)) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Coordinate);
		}

		public override bool Matches(Tile Object)
		{
			if (Object == null) return false;
			return Object.Coordinate == Coordinate;
		}
	}
}
