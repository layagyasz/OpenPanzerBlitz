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

		public override bool IsTransient { get; } = false;

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

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((Serializable)Zone);
		}

		public override bool Matches(Tile Object)
		{
			if (Object == null) return false;
			return Zone.ContainsPoint(new Vector2f(Object.Coordinate.X, Object.Coordinate.Y));
		}
	}
}
