﻿using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class RatioObjective : Objective
	{
		enum Attribute { NUMERATOR, DENOMINATOR }

		public readonly Objective Numerator;
		public readonly Objective Denominator;

		public RatioObjective(Objective Numerator, Objective Denominator)
		{
			this.Numerator = Numerator;
			this.Denominator = Denominator;
		}

		public RatioObjective(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Numerator = (Objective)attributes[(int)Attribute.NUMERATOR];
			Denominator = (Objective)attributes[(int)Attribute.DENOMINATOR];
		}

		public RatioObjective(SerializationInputStream Stream)
		{
			Numerator = (Objective)ObjectiveSerializer.Instance.Deserialize(Stream);
			Denominator = (Objective)ObjectiveSerializer.Instance.Deserialize(Stream);
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			ObjectiveSerializer.Instance.Serialize(Numerator, Stream);
			ObjectiveSerializer.Instance.Serialize(Denominator, Stream);
		}

		public override bool CanStopEarly()
		{
			return Numerator.CanStopEarly() || Denominator.CanStopEarly();
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			return Numerator.GetScore(ForArmy, Match, Cache) / Denominator.GetScore(ForArmy, Match, Cache);
		}

		public override int? GetMaximumScore(Objective Objective, Army ForArmy, Match Match)
		{
			if (Objective == this) Objective = Numerator;
			return Numerator.GetMaximumScore(Objective, ForArmy, Match);
		}

		public override IEnumerable<Tile> GetTiles(Map Map)
		{
			return Numerator.GetTiles(Map).Concat(Denominator.GetTiles(Map));
		}
	}
}
