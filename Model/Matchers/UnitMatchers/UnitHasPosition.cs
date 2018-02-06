using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitHasPosition : Matcher<Unit>
	{
		enum Attribute { MATCHER }

		public readonly Matcher<Tile> Matcher;

		public UnitHasPosition(Matcher<Tile> Matcher)
		{
			this.Matcher = Matcher;
		}

		public UnitHasPosition(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			Matcher = (Matcher<Tile>)attributes[(int)Attribute.MATCHER];
		}

		public UnitHasPosition(SerializationInputStream Stream)
			: this((Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream)) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
		}

		public bool Matches(Unit Unit)
		{
			return Matcher.Matches(Unit.Position);
		}

		public IEnumerable<Matcher<Unit>> Flatten()
		{
			yield return this;
		}
	}
}
