using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class RatioObjective : Objective
	{
		enum Attribute { NUMERATOR, DENOMINATOR }

		public readonly Objective Numerator;
		public readonly Objective Denominator;

		public RatioObjective(string UniqueKey, Objective Numerator, Objective Denominator)
			: base(UniqueKey)
		{
			this.Numerator = Numerator;
			this.Denominator = Denominator;
		}

		public RatioObjective(ParseBlock Block)
			: base(Block.Name)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Numerator = (Objective)attributes[(int)Attribute.NUMERATOR];
			Denominator = (Objective)attributes[(int)Attribute.DENOMINATOR];
		}

		public RatioObjective(SerializationInputStream Stream)
			: base(Stream)
		{
			Numerator = (Objective)ObjectiveSerializer.Instance.Deserialize(Stream);
			Denominator = (Objective)ObjectiveSerializer.Instance.Deserialize(Stream);
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			ObjectiveSerializer.Instance.Serialize(Numerator, Stream);
			ObjectiveSerializer.Instance.Serialize(Denominator, Stream);
		}

		public override int CalculateScore(Army ForArmy, Match Match)
		{
			return Numerator.CalculateScore(ForArmy, Match) / Denominator.CalculateScore(ForArmy, Match);
		}
	}
}
