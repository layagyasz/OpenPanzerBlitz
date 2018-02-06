using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class CompositeObjective : Objective
	{
		public static readonly Func<bool, bool, bool> AND = (i, j) => i && j;
		public static readonly Func<bool, bool, bool> OR = (i, j) => i || j;

		protected static readonly Func<bool, bool, bool>[] AGGREGATORS = { AND, OR };

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
				AGGREGATORS[Stream.ReadByte()])
		{ }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Objectives, i => ObjectiveSerializer.Instance.Serialize(i, Stream));
			Stream.Write((byte)Array.IndexOf(AGGREGATORS, Aggregator));
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			bool seed = Objectives.First().GetScore(ForArmy, Match, Cache) != 0;
			return Objectives.Skip(1).Select(i => i.GetScore(ForArmy, Match, Cache) != 0).Aggregate(seed, Aggregator)
							 ? 1 : 0;
		}

		public override IEnumerable<Tile> GetTiles(Map Map)
		{
			return Objectives.SelectMany(i => i.GetTiles(Map));
		}
	}
}
