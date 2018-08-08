using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class HighestUniqueScoreObjective : Objective
	{
		public HighestUniqueScoreObjective() { }

		public HighestUniqueScoreObjective(ParseBlock Block) { }

		public HighestUniqueScoreObjective(SerializationInputStream Stream) { }

		public override void Serialize(SerializationOutputStream Stream) { }

		public override bool CanStopEarly()
		{
			return false;
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			var score = GenericScoreArmy(ForArmy, Match, Cache);
			var compareScore = Match.Armies
									.Where(i => i.Configuration.Team != ForArmy.Configuration.Team)
									.Max(i => GenericScoreArmy(ForArmy, Match, Cache));
			return score > compareScore ? 1 : 0;
		}

		public override int? GetMaximumScore(Objective Objective, Army ForArmy, Match Match)
		{
			if (Objective != this) return null;
			return ForArmy.Configuration.VictoryCondition.Scorers
						  .Where(i => i is PointsObjective)
						  .Sum(i => i.GetMaximumScore(Objective, ForArmy, Match));
		}

		public override IEnumerable<Tile> GetTiles(Map Map)
		{
			return Enumerable.Empty<Tile>();
		}

		int GenericScoreArmy(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			return ForArmy.Configuration.VictoryCondition.Scorers
						  .Where(i => i is PointsObjective)
						  .Sum(i => i.GetScore(ForArmy, Match, Cache));
		}
	}
}
