using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class SumObjective : Objective
	{
		public readonly List<Objective> Objectives;

		public SumObjective(string UniqueKey, IEnumerable<Objective> Objectives)
			: base(UniqueKey)
		{
			this.Objectives = Objectives.ToList();
		}

		public SumObjective(ParseBlock Block)
			: base(Block.Name)
		{
			Objectives = Block.BreakToList<Objective>();
		}

		public SumObjective(SerializationInputStream Stream)
			: base(Stream)
		{
			Objectives = Stream.ReadEnumerable(
				i => (Objective)ObjectiveSerializer.Instance.Deserialize(Stream)).ToList();
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			Stream.Write(Objectives, i => ObjectiveSerializer.Instance.Serialize(i, Stream));
		}

		public override int CalculateScore(Army ForArmy, Match Match)
		{
			_Score = Objectives.Sum(i => i.CalculateScore(ForArmy, Match));
			return _Score;
		}
	}
}
