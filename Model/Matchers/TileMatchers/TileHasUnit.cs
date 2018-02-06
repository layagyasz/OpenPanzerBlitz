using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileHasUnit : Matcher<Tile>
	{
		enum Attribute { MATCHER };

		public readonly Matcher<Unit> Matcher;

		public TileHasUnit(Matcher<Unit> Matcher)
		{
			this.Matcher = Matcher;
		}

		public TileHasUnit(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			Matcher = (Matcher<Unit>)attributes[(int)Attribute.MATCHER];
		}

		public TileHasUnit(SerializationInputStream Stream)
			: this((Matcher<Unit>)MatcherSerializer.Instance.Deserialize(Stream)) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
		}

		public bool Matches(Tile Tile)
		{
			return Tile.Units.Any(i => Matcher.Matches(i));
		}

		public IEnumerable<Matcher<Tile>> Flatten()
		{
			yield return this;
		}
	}
}
