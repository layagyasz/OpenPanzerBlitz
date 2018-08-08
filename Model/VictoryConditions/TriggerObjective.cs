using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TriggerObjective : Objective
	{
		enum Attribute { OBJECTIVE, THRESHOLD, INVERT };

		public readonly Objective Objective;
		public readonly int Threshold;
		public readonly bool Invert;

		public TriggerObjective(Objective Objective, int Threshold, bool Invert)
		{
			this.Objective = Objective;
			this.Threshold = Threshold;
			this.Invert = Invert;
		}

		public TriggerObjective(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Objective = (Objective)attributes[(int)Attribute.OBJECTIVE];
			Threshold = (int)attributes[(int)Attribute.THRESHOLD];
			Invert = (bool)(attributes[(int)Attribute.INVERT] ?? false);
		}

		public TriggerObjective(SerializationInputStream Stream)
			: this(
				(Objective)ObjectiveSerializer.Instance.Deserialize(Stream),
				Stream.ReadInt32(),
				Stream.ReadBoolean())
		{ }

		public override void Serialize(SerializationOutputStream Stream)
		{
			ObjectiveSerializer.Instance.Serialize(Objective, Stream);
			Stream.Write(Threshold);
			Stream.Write(Invert);
		}

		public override bool CanStopEarly()
		{
			return Objective.CanStopEarly();
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			bool t = Invert ? Objective.GetScore(ForArmy, Match, Cache) <= Threshold
									   : Objective.GetScore(ForArmy, Match, Cache) >= Threshold;
			return t ? 1 : 0;
		}

		public override int? GetMaximumScore(Objective Objective, Army ForArmy, Match Match)
		{
			if (Objective == this) return 1;
			var score = this.Objective.GetMaximumScore(Objective, ForArmy, Match);
			if (score == null) return null;
			if (Objective == this.Objective) return Math.Min(Threshold, (int)score);
			return score;
		}

		public override IEnumerable<Tile> GetTiles(Map Map)
		{
			return Objective.GetTiles(Map);
		}
	}
}
