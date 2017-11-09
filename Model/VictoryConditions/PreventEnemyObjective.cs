﻿using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class PreventEnemyObjective : Objective
	{
		public PreventEnemyObjective() { }

		public PreventEnemyObjective(ParseBlock Block) { }

		public PreventEnemyObjective(SerializationInputStream Stream) { }

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			return Match.Armies.Where(i => i.Configuration.Team != ForArmy.Configuration.Team)
						  .All(i => i.GetObjectiveSuccessLevel(Match) == ObjectiveSuccessLevel.DEFEAT) ? 1 : 0;
		}

		public override void Serialize(SerializationOutputStream Stream) { }
	}
}
