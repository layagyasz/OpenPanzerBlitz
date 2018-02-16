using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ObjectiveSuccessTrigger : Serializable
	{
		enum Attribute { SUCCESS_LEVEL, THRESHOLD, INVERT, OBJECTIVE }

		public readonly ObjectiveSuccessLevel SuccessLevel;
		public readonly int Threshold;
		public readonly bool Invert;
		public readonly Objective Objective;

		public readonly bool CanStopEarly;

		public ObjectiveSuccessTrigger(
			ObjectiveSuccessLevel SuccessLevel, int Threshold, bool Invert, Objective Objective)
		{
			this.SuccessLevel = SuccessLevel;
			this.Threshold = Threshold;
			this.Invert = Invert;
			this.Objective = Objective;

			CanStopEarly = Objective.CanStopEarly();
		}

		public ObjectiveSuccessTrigger(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			SuccessLevel = (ObjectiveSuccessLevel)attributes[(int)Attribute.SUCCESS_LEVEL];
			Threshold = Parse.DefaultIfNull(attributes[(int)Attribute.THRESHOLD], 1);
			Invert = Parse.DefaultIfNull(attributes[(int)Attribute.INVERT], false);
			Objective = (Objective)attributes[(int)Attribute.OBJECTIVE];

			CanStopEarly = Objective.CanStopEarly();
		}

		public ObjectiveSuccessTrigger(SerializationInputStream Stream)
			: this(
				(ObjectiveSuccessLevel)Stream.ReadByte(),
				Stream.ReadInt32(),
				Stream.ReadBoolean(),
				(Objective)Stream.ReadObject(i => ObjectiveSerializer.Instance.Deserialize(Stream), false, true))
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)SuccessLevel);
			Stream.Write(Threshold);
			Stream.Write(Invert);
			Stream.Write(Objective, i => ObjectiveSerializer.Instance.Serialize(Objective, Stream), false, true);
		}

		public bool IsSatisfied(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			return Invert ? Objective.GetScore(ForArmy, Match, Cache) <= Threshold
										 : Objective.GetScore(ForArmy, Match, Cache) >= Threshold;
		}
	}
}
