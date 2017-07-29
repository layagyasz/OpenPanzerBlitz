using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitsDestroyed : Objective
	{
		enum Attribute { FRIENDLY, OVERRIDE_SCORES };

		bool _Friendly;
		Dictionary<UnitConfiguration, int> _OverrideScores;
		int _Score;

		public UnitsDestroyed(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));


			_Friendly = Parse.DefaultIfNull(attributes[(int)Attribute.FRIENDLY], false);
			if (attributes[(int)Attribute.OVERRIDE_SCORES] != null)
			{
				_OverrideScores = ((List<Tuple<object, object>>)attributes[(int)Attribute.OVERRIDE_SCORES])
					.ToDictionary(i => (UnitConfiguration)i.Item1, i => (int)i.Item2);
			}
			else _OverrideScores = new Dictionary<UnitConfiguration, int>();
		}

		public int CalculateScore(Army ForArmy, Match Match)
		{
			IEnumerable<Unit> countedUnits =
				Match.Armies.Where(i => _Friendly == (i.Configuration.Team == ForArmy.Configuration.Team))
					 .SelectMany(i => i.Units)
					 .Where(i => i.Destroyed);
			_Score = countedUnits.Sum(
				i => _OverrideScores.ContainsKey(i.Configuration) ? _OverrideScores[i.Configuration] : 1);
			return _Score;
		}

		public int GetScore()
		{
			return _Score;
		}
	}
}
