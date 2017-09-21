using System;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class PreventEnemyObjective : Objective
	{
		public PreventEnemyObjective(string UniqueKey)
			: base(UniqueKey) { }

		public PreventEnemyObjective(ParseBlock Block)
			: base(Block.Name) { }

		public PreventEnemyObjective(SerializationInputStream Stream)
			: base(Stream) { }

		public override int CalculateScore(Army ForArmy, Match Match)
		{
			_Score = Match.Armies.Where(i => i.Configuration.Team != ForArmy.Configuration.Team)
						  .All(i => i.GetObjectiveSuccessLevel(Match) == ObjectiveSuccessLevel.DEFEAT) ? 1 : 0;
			return _Score;
		}
	}
}
