using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class CompositeMatcher<T> : Matcher<T>
	{
		public static readonly Func<bool, bool, bool> AND = (i, j) => i && j;
		public static readonly Func<bool, bool, bool> OR = (i, j) => i || j;

		protected static readonly Func<bool, bool, bool>[] AGGREGATORS = { AND, OR };

		public readonly List<Matcher<T>> Matchers;
		public readonly Func<bool, bool, bool> Aggregator;

		public CompositeMatcher(IEnumerable<Matcher<T>> Matchers, Func<bool, bool, bool> Aggregator)
		{
			this.Matchers = Matchers.ToList();
			this.Aggregator = Aggregator;
		}

		public CompositeMatcher(ParseBlock Block, Func<bool, bool, bool> Aggregator)
		{
			Matchers = Block.BreakToList<Matcher<T>>();
			this.Aggregator = Aggregator;
		}

		public CompositeMatcher(SerializationInputStream Stream)
					: this(
				Stream.ReadEnumerable(i => (Matcher<T>)MatcherSerializer.Instance.Deserialize(Stream)).ToList(),
				AGGREGATORS[Stream.ReadByte()])
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Matchers, i => MatcherSerializer.Instance.Serialize(i, Stream));
			Stream.Write((byte)Array.IndexOf(AGGREGATORS, Aggregator));
		}

		public bool Matches(T Object)
		{
			bool seed = Matchers.First().Matches(Object);
			return Matchers.Skip(1).Select(i => i.Matches(Object)).Aggregate(seed, Aggregator);
		}
	}
}
