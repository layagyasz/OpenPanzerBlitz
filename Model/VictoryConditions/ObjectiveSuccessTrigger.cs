using System;
using System.Collections.Generic;
using System.Linq;

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

		public ObjectiveSuccessTrigger(
			ObjectiveSuccessLevel SuccessLevel, int Threshold, bool Invert, Objective Objective)
		{
			this.SuccessLevel = SuccessLevel;
			this.Threshold = Threshold;
			this.Invert = Invert;
			this.Objective = Objective;
		}

		public ObjectiveSuccessTrigger(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			SuccessLevel = (ObjectiveSuccessLevel)attributes[(int)Attribute.SUCCESS_LEVEL];
			Threshold = Parse.DefaultIfNull(attributes[(int)Attribute.THRESHOLD], 1);
			Invert = Parse.DefaultIfNull(attributes[(int)Attribute.INVERT], false);
			Objective = (Objective)attributes[(int)Attribute.OBJECTIVE];
		}

		public ObjectiveSuccessTrigger(SerializationInputStream Stream)
		{
			SuccessLevel = (ObjectiveSuccessLevel)Stream.ReadByte();
			Threshold = Stream.ReadInt32();
			Invert = Stream.ReadBoolean();
			Objective = (Objective)Stream.ReadObject(
				i => ObjectiveSerializer.Instance.Deserialize(Stream), false, true);
		}

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
