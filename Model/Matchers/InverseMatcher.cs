using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class InverseMatcher<T> : Matcher<T>
	{
		enum Attribute { MATCHER };

		public readonly Matcher<T> Matcher;

		public override bool IsTransient
		{
			get
			{
				return Matcher.IsTransient;
			}
		}

		public InverseMatcher(Matcher<T> Matcher)
		{
			this.Matcher = Matcher;
		}

		public InverseMatcher(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Matcher = (Matcher<T>)attributes[(int)Attribute.MATCHER];
		}

		public InverseMatcher(SerializationInputStream Stream)
			: this((Matcher<T>)MatcherSerializer.Instance.Deserialize(Stream)) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
		}

		public override bool Matches(T Object)
		{
			return !Matcher.Matches(Object);
		}

		public override IEnumerable<Matcher<T>> Flatten()
		{
			yield return Matcher;
		}
	}
}
