using System.Collections.Generic;

using Cardamom.Planar;
using Cardamom.Serialization;

using SFML.Window;

namespace PanzerBlitz
{
	public class TileWithin : Matcher<Tile>
	{
		enum Attribute { ZONE };

		public readonly Polygon Zone;

		public TileWithin(Polygon Zone)
		{
			this.Zone = Zone;
		}

		public TileWithin(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Zone = (Polygon)attributes[(int)Attribute.ZONE];
		}

		public TileWithin(SerializationInputStream Stream)
			: this(new Polygon(Stream)) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((Serializable)Zone);
		}

		public bool Matches(Tile Tile)
		{
			if (Tile == null) return false;
			return Zone.ContainsPoint(new Vector2f(Tile.Coordinate.X, Tile.Coordinate.Y));
		}

		public IEnumerable<Matcher<Tile>> Flatten()
		{
			yield return this;
		}
	}
}
