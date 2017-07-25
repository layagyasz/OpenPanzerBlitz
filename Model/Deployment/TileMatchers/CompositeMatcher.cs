using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class CompositeMatcher : Matcher
	{
		public readonly List<Matcher> Matchers;
		public readonly Func<bool, bool, bool> Aggregator;

		public CompositeMatcher(ParseBlock Block, Func<bool, bool, bool> Aggregator)
		{
			Matchers = Block.BreakToList<Matcher>();
			this.Aggregator = Aggregator;
		}

		public bool Matches(Tile Tile)
		{
			bool seed = Matchers.First().Matches(Tile);
			return Matchers.Skip(1).Select(i => i.Matches(Tile)).Aggregate(seed, Aggregator);
		}
	}
}
