using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class VictoryCondition : Serializable
	{
		enum Attribute { OBJECTIVES, TRIGGERS, STOP_EARLY };

		public readonly List<Objective> Scorers;
		public readonly List<ObjectiveSuccessTrigger> Triggers;
		public readonly bool StopEarly;

		public VictoryCondition(
			IEnumerable<Objective> Scorers, IEnumerable<ObjectiveSuccessTrigger> Triggers, bool StopEarly = false)
		{
			this.Scorers = Scorers.ToList();
			this.Triggers = Triggers.ToList();
			this.StopEarly = StopEarly;
		}

		public VictoryCondition(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute), true);
			Scorers = ((Dictionary<string, object>)attributes[(int)Attribute.OBJECTIVES])
				.Values.Cast<Objective>().ToList();
			Triggers = (List<ObjectiveSuccessTrigger>)attributes[(int)Attribute.TRIGGERS];
			StopEarly = Parse.DefaultIfNull(attributes[(int)Attribute.STOP_EARLY], false);
		}

		public VictoryCondition(SerializationInputStream Stream)
		{
			Scorers = Stream.ReadEnumerable(
				i => (Objective)ObjectiveSerializer.Instance.Deserialize(Stream, false, true)).ToList();
			Triggers = Stream.ReadEnumerable(i => new ObjectiveSuccessTrigger(Stream)).ToList();
			StopEarly = Stream.ReadBoolean();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Scorers, i => ObjectiveSerializer.Instance.Serialize(i, Stream, false, true));
			Stream.Write(Triggers);
			Stream.Write(StopEarly);
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
