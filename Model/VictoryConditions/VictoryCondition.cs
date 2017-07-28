using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class VictoryCondition
	{
		enum Attribute { OBJECTIVES, TRIGGERS };

		public readonly List<Objective> Scorers;
		public readonly List<ObjectiveSuccessTrigger> Triggers;

		public VictoryCondition(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute), true);
			Scorers = ((Dictionary<string, object>)attributes[(int)Attribute.OBJECTIVES])
				.Values.Cast<Objective>().ToList();
			Triggers = (List<ObjectiveSuccessTrigger>)attributes[(int)Attribute.TRIGGERS];
		}

		public ObjectiveSuccessLevel GetMatchResult(Army ForArmy, Match Match)
		{
			Scorers.ForEach(i => i.CalculateScore(ForArmy, Match));

			ObjectiveSuccessLevel result = ObjectiveSuccessLevel.DEFEAT;
			foreach (ObjectiveSuccessTrigger t in Triggers)
			{
				if (t.IsSatisfied() && (int)t.SuccessLevel > (int)result) result = t.SuccessLevel;
			}
			return result;
		}
	}
}
