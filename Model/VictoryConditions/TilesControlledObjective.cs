﻿using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TilesControlledObjective : Objective
	{
		enum Attribute { FRIENDLY, MATCHER };

		public readonly bool Friendly;
		public readonly Matcher<Tile> Matcher;

		public TilesControlledObjective(bool Friendly, Matcher<Tile> Matcher)
		{
			this.Friendly = Friendly;
			this.Matcher = Matcher;
		}

		public TilesControlledObjective(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Friendly = (bool)attributes[(int)Attribute.FRIENDLY];
			Matcher = (Matcher<Tile>)attributes[(int)Attribute.MATCHER];
		}

		public TilesControlledObjective(SerializationInputStream Stream)
			: this(
				Stream.ReadBoolean(),
				(Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream))
		{ }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Friendly);
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
		}

		public override bool CanStopEarly()
		{
			return false;
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			return Match.Map.TilesEnumerable.Count(
				i => Friendly == (i.ControllingArmy == ForArmy) && Matcher.Matches(i));
		}

		public override int? GetMaximumScore(Objective Objective, Army ForArmy, Match Match)
		{
			return Objective == this ? (int?)Match.Map.TilesEnumerable.Count(Matcher.MatchesTransient) : null;
		}

		public override IEnumerable<Tile> GetTiles(Map Map)
		{
			return Map.TilesEnumerable.Where(i => Matcher.Matches(i));
		}
	}
}
