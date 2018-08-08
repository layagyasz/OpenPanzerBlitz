using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class CompositeObjective : Objective
	{
		public readonly List<Objective> Objectives;
		public readonly Func<bool, bool, bool> Aggregator;

		public CompositeObjective(IEnumerable<Objective> Objectives, Func<bool, bool, bool> Aggregator)
		{
			this.Objectives = Objectives.ToList();
			this.Aggregator = Aggregator;
		}

		public CompositeObjective(ParseBlock Block, Func<bool, bool, bool> Aggregator)
		{
			Objectives = Block.BreakToList<Objective>();
			this.Aggregator = Aggregator;
		}

		public CompositeObjective(SerializationInputStream Stream)
			: this(
				Stream.ReadEnumerable(i => (Objective)ObjectiveSerializer.Instance.Deserialize(Stream)).ToList(),
				Aggregators.AGGREGATORS[Stream.ReadByte()])
		{ }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Objectives, i => ObjectiveSerializer.Instance.Serialize(i, Stream));
			Stream.Write((byte)Array.IndexOf(Aggregators.AGGREGATORS, Aggregator));
		}

		public override bool CanStopEarly()
		{
			return Objectives.Any(i => i.CanStopEarly());
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			bool seed = Objectives.First().GetScore(ForArmy, Match, Cache) != 0;
			return Objectives.Skip(1).Select(i => i.GetScore(ForArmy, Match, Cache) != 0).Aggregate(seed, Aggregator)
							 ? 1 : 0;
		}

		public override int? GetMaximumScore(Objective Objective, Army ForArmy, Match Match)
		{
			if (Objective == this) return 1;
			foreach (var objective in Objectives)
			{
				var score = objective.GetMaximumScore(Objective, ForArmy, Match);
				if (score != null) return score;
			}
			return null;
		}

		public override IEnumerable<Tile> GetTiles(Map Map)
		{
			return Objectives.SelectMany(i => i.GetTiles(Map));
		}
	}
}
