using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class SumObjective : Objective
	{
		public readonly List<Objective> Objectives;

		public SumObjective(IEnumerable<Objective> Objectives)
		{
			this.Objectives = Objectives.ToList();
		}

		public SumObjective(ParseBlock Block)
		{
			Objectives = Block.BreakToList<Objective>();
		}

		public SumObjective(SerializationInputStream Stream)
		{
			Objectives = Stream.ReadEnumerable(
				i => (Objective)ObjectiveSerializer.Instance.Deserialize(Stream)).ToList();
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Objectives, i => ObjectiveSerializer.Instance.Serialize(i, Stream));
		}

		public override bool CanStopEarly()
		{
			return Objectives.Any(i => i.CanStopEarly());
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			return Objectives.Sum(i => i.GetScore(ForArmy, Match, Cache));
		}

		public override IEnumerable<Tile> GetTiles(Map Map)
		{
			return Objectives.SelectMany(i => i.GetTiles(Map));
		}
	}
}
