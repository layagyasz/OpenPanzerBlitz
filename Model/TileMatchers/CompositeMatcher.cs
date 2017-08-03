using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class CompositeMatcher : Matcher
	{
		public static readonly Func<bool, bool, bool> AND = (i, j) => i && j;
		public static readonly Func<bool, bool, bool> OR = (i, j) => i || j;

		static readonly Func<bool, bool, bool>[] AGGREGATORS = { AND, OR };

		public readonly List<Matcher> Matchers;
		public readonly Func<bool, bool, bool> Aggregator;

		public CompositeMatcher(IEnumerable<Matcher> Matchers, Func<bool, bool, bool> Aggregator)
		{
			this.Matchers = Matchers.ToList();
			this.Aggregator = Aggregator;
		}

		public CompositeMatcher(ParseBlock Block, Func<bool, bool, bool> Aggregator)
		{
			Matchers = Block.BreakToList<Matcher>();
			this.Aggregator = Aggregator;
		}

		public CompositeMatcher(SerializationInputStream Stream)
			: this(
				Stream.ReadEnumerable(i => MatcherSerializer.Deserialize(Stream)),
				AGGREGATORS[Stream.ReadByte()])
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Matchers);
			Stream.Write((byte)Array.IndexOf(AGGREGATORS, Aggregator));
		}

		public bool Matches(Tile Tile)
		{
			bool seed = Matchers.First().Matches(Tile);
			return Matchers.Skip(1).Select(i => i.Matches(Tile)).Aggregate(seed, Aggregator);
		}
	}
}
