using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class InverseMatcher<T> : Matcher<T>
	{
		enum Attribute { MATCHER };

		public readonly Matcher<T> Matcher;

		public InverseMatcher(Matcher<T> Matcher)
		{
			this.Matcher = Matcher;
		}

		public InverseMatcher(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Matcher = (Matcher<T>)attributes[(int)Attribute.MATCHER];
		}

		public InverseMatcher(SerializationInputStream Stream)
			: this((Matcher<T>)MatcherSerializer.Instance.Deserialize(Stream)) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
		}

		public bool Matches(T Object)
		{
			return !Matcher.Matches(Object);
		}
	}
}
