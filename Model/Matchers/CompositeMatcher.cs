using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class CompositeMatcher<T> : Matcher<T>
	{
		public readonly List<Matcher<T>> Matchers;
		public readonly Func<bool, bool, bool> Aggregator;

		public override bool IsTransient
		{
			get
			{
				return Matchers.Any(i => i.IsTransient);
			}
		}

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
				Aggregators.AGGREGATORS[Stream.ReadByte()])
		{ }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Matchers, i => MatcherSerializer.Instance.Serialize(i, Stream));
			Stream.Write((byte)Array.IndexOf(Aggregators.AGGREGATORS, Aggregator));
		}

		public override bool Matches(T Object)
		{
			var seed = Matchers.First().Matches(Object);
			return Matchers.Skip(1).Select(i => i.Matches(Object)).Aggregate(seed, Aggregator);
		}

		public override IEnumerable<Matcher<T>> Flatten()
		{
			return Matchers.SelectMany(i => i.Flatten());
		}
	}
}
