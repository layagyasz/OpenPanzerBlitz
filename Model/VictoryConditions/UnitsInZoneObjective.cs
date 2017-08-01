using System;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitsInZoneObjective : Objective
	{
		enum Attribute { MATCHER, FRIENDLY }

		public readonly Matcher Matcher;
		public readonly bool Friendly;

		int _Score;

		public UnitsInZoneObjective(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Matcher = (Matcher)attributes[(int)Attribute.MATCHER];
			Friendly = Parse.DefaultIfNull(attributes[(int)Attribute.FRIENDLY], true);
		}

		public int CalculateScore(Army ForArmy, Match Match)
		{
			_Score = Match.Armies.Where(
				i => Friendly == (i.Configuration.Team == ForArmy.Configuration.Team))
						  .SelectMany(i => i.Units)
						  .Count(i => i.Position != null
								 && !i.Configuration.IsNeutral()
								 && (Matcher == null || Matcher.Matches(i.Position)));
			return _Score;
		}

		public int GetScore()
		{
			return _Score;
		}
	}
}
