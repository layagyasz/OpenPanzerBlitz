using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class VictoryCondition : Serializable
	{
		enum Attribute { OBJECTIVES, TRIGGERS };

		public readonly List<Objective> Scorers;
		public readonly List<ObjectiveSuccessTrigger> Triggers;

		public VictoryCondition(IEnumerable<Objective> Scorers, IEnumerable<ObjectiveSuccessTrigger> Triggers)
		{
			this.Scorers = Scorers.ToList();
			this.Triggers = Triggers.ToList();
		}

		public VictoryCondition(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute), true);
			Scorers = ((Dictionary<string, object>)attributes[(int)Attribute.OBJECTIVES])
				.Values.Cast<Objective>().ToList();
			Triggers = (List<ObjectiveSuccessTrigger>)attributes[(int)Attribute.TRIGGERS];
		}

		public VictoryCondition(SerializationInputStream Stream)
		{
			Scorers = Stream.ReadEnumerable(
				i => (Objective)ObjectiveSerializer.Instance.Deserialize(Stream, false, true)).ToList();
			Triggers = Stream.ReadEnumerable(i => new ObjectiveSuccessTrigger(Stream)).ToList();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Scorers, i => ObjectiveSerializer.Instance.Serialize(i, Stream, false, true));
			Stream.Write(Triggers);
		}

		public ObjectiveSuccessLevel GetMatchResult(Army ForArmy, Match Match)
		{
			Dictionary<Objective, int> cache = new Dictionary<Objective, int>();

			ObjectiveSuccessLevel result = ObjectiveSuccessLevel.DEFEAT;
			foreach (ObjectiveSuccessTrigger t in Triggers)
			{
				if (t.IsSatisfied(ForArmy, Match, cache) && (int)t.SuccessLevel > (int)result) result = t.SuccessLevel;
			}
			return result;
		}
	}
}
