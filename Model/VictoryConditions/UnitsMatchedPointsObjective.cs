using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitsMatchedPointsObjective : Objective
	{
		enum Attribute { FRIENDLY, MATCHER };

		public readonly Matcher<Unit> Matcher;
		public readonly bool Friendly;

		public UnitsMatchedPointsObjective(Matcher<Unit> Matcher, bool Friendly)
		{
			this.Matcher = Matcher;
			this.Friendly = Friendly;
		}

		public UnitsMatchedPointsObjective(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Matcher = (Matcher<Unit>)attributes[(int)Attribute.MATCHER];
			Friendly = (bool)attributes[(int)Attribute.FRIENDLY];
		}

		public UnitsMatchedPointsObjective(SerializationInputStream Stream)
			: this(
				(Matcher<Unit>)MatcherSerializer.Instance.Deserialize(Stream),
				Stream.ReadBoolean())
		{ }

		public override void Serialize(SerializationOutputStream Stream)
		{
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
			Stream.Write(Friendly);
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			return (int)(Match.Armies
						 .Where(i => Friendly == (i.Configuration.Team == ForArmy.Configuration.Team))
						 .SelectMany(i => i.Units)
						 .Where(Matcher.Matches)
						 .Sum(i => i.Configuration.GetPointValue()) * 100);
		}
	}
}
