using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class PointsObjective : Objective
	{
		enum Attribute { OBJECTIVE, POINTS }

		public readonly Objective Objective;
		public readonly int Points;

		public PointsObjective(Objective Objective, int Points)
		{
			this.Objective = Objective;
			this.Points = Points;
		}

		public PointsObjective(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Objective = (Objective)attributes[(int)Attribute.OBJECTIVE];
			Points = (int)(attributes[(int)Attribute.POINTS] ?? 1);
		}

		public PointsObjective(SerializationInputStream Stream)
			: this((Objective)ObjectiveSerializer.Instance.Deserialize(Stream), Stream.ReadInt32()) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			ObjectiveSerializer.Instance.Serialize(Objective, Stream);
			Stream.Write(Points);
		}

		public override bool CanStopEarly()
		{
			return Objective.CanStopEarly();
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			return Points * Objective.GetScore(ForArmy, Match, Cache);
		}

		public override int? GetMaximumScore(Objective Objective, Army ForArmy, Match Match)
		{
			if (Objective == this) Objective = this.Objective;
			var score = this.Objective.GetMaximumScore(Objective, ForArmy, Match);
			return score == null ? null : Points * score;
		}

		public override IEnumerable<Tile> GetTiles(Map Map)
		{
			return Objective.GetTiles(Map);
		}
	}
}
