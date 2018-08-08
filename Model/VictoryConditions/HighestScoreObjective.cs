using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class HighestScoreObjective : Objective
	{
		enum Attribute { METRIC }

		public readonly Objective Metric;

		public HighestScoreObjective(Objective Metric)
		{
			this.Metric = Metric;
		}

		public HighestScoreObjective(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Metric = (Objective)attributes[(int)Attribute.METRIC];
		}

		public HighestScoreObjective(SerializationInputStream Stream)
			: this((Objective)ObjectiveSerializer.Instance.Deserialize(Stream)) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			ObjectiveSerializer.Instance.Serialize(Metric, Stream);
		}

		public override bool CanStopEarly()
		{
			if (Metric == null) return false;
			return Metric.CanStopEarly();
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			var score = Metric.GetScore(ForArmy, Match, Cache);
			var compareScore = Match.Armies
									.Where(i => i.Configuration.Team != ForArmy.Configuration.Team)
									.Max(i => Metric.CalculateScore(i, Match, new Dictionary<Objective, int>()));
			return score > compareScore ? 1 : 0;
		}

		public override int? GetMaximumScore(Objective Objective, Army ForArmy, Match Match)
		{
			if (Objective == this) return 1;
			return Metric.GetMaximumScore(Objective, ForArmy, Match);
		}

		public override IEnumerable<Tile> GetTiles(Map Map)
		{
			return Metric.GetTiles(Map);
		}
	}
}
