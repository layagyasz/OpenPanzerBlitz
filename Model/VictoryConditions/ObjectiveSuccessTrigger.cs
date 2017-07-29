using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ObjectiveSuccessTrigger
	{
		enum Attribute { SUCCESS_LEVEL, THRESHOLD, INVERT, OBJECTIVE }

		public readonly ObjectiveSuccessLevel SuccessLevel;
		public readonly int Threshold;
		public readonly bool Invert;
		public readonly Objective Objective;

		public ObjectiveSuccessTrigger(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			SuccessLevel = (ObjectiveSuccessLevel)attributes[(int)Attribute.SUCCESS_LEVEL];
			Threshold = Parse.DefaultIfNull(attributes[(int)Attribute.THRESHOLD], 1);
			Invert = Parse.DefaultIfNull(attributes[(int)Attribute.INVERT], false);
			Objective = (Objective)attributes[(int)Attribute.OBJECTIVE];
		}

		public bool IsSatisfied()
		{
			return Invert ? Objective.GetScore() <= Threshold : Objective.GetScore() >= Threshold;
		}
	}
}
