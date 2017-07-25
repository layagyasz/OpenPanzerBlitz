using System;

using Cardamom.Planar;
using Cardamom.Serialization;

using SFML.Window;

namespace PanzerBlitz
{
	public class TileWithin : Matcher
	{
		enum Attribute { ZONE };

		public readonly Polygon Zone;

		public TileWithin(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			Zone = (Polygon)attributes[(int)Attribute.ZONE];
		}

		public bool Matches(Tile Tile)
		{
			return Zone.ContainsPoint(new Vector2f(Tile.X, Tile.Y));
		}
	}
}
