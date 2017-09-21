using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitPositionMatches : Matcher<Unit>
	{
		enum Attribute { MATCHER }

		public readonly Matcher<Tile> Matcher;

		public UnitPositionMatches(Matcher<Tile> Matcher)
		{
			this.Matcher = Matcher;
		}

		public UnitPositionMatches(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			Matcher = (Matcher<Tile>)attributes[(int)Attribute.MATCHER];
		}

		public UnitPositionMatches(SerializationInputStream Stream)
			: this((Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream)) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
		}

		public bool Matches(Unit Unit)
		{
			return Matcher.Matches(Unit.Position);
		}
	}
}
