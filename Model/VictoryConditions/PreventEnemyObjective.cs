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

		public override bool CanStopEarly()
		{
			return false;
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			return Match.Armies.Where(i => i.Configuration.Team != ForArmy.Configuration.Team)
						.All(i => i.GetObjectiveSuccessLevel() < ObjectiveSuccessLevel.VICTORY) ? 1 : 0;
		}

		public override int? GetMaximumScore(Objective Objective, Army ForArmy, Match Match)
		{
			return Objective == this ? (int?)1 : null;
		}

		public override IEnumerable<Tile> GetTiles(Map Map)
		{
			return Enumerable.Empty<Tile>();
		}

		public override void Serialize(SerializationOutputStream Stream) { }
	}
}
