using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ObjectiveSuccessTrigger
	{
		enum Attribute { SUCCESS_LEVEL, THRESHOLD, OBJECTIVE }

		public readonly ObjectiveSuccessLevel SuccessLevel;
		public readonly int Threshold;
		public readonly Objective Objective;

		public ObjectiveSuccessTrigger(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			SuccessLevel = (ObjectiveSuccessLevel)attributes[(int)Attribute.SUCCESS_LEVEL];
			Threshold = Parse.DefaultIfNull(attributes[(int)Attribute.THRESHOLD], 1);
			Objective = (Objective)attributes[(int)Attribute.OBJECTIVE];
		}

		public bool IsSatisfied()
		{
			return Objective.GetScore() >= Threshold;
		}
	}
}
