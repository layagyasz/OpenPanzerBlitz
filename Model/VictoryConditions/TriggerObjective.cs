using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TriggerObjective : Objective
	{
		enum Attribute { OBJECTIVE, THRESHOLD, INVERT };

		public readonly Objective Objective;
		public readonly int Threshold;
		public readonly bool Invert;

		public TriggerObjective(string UniqueKey, Objective Objective, int Threshold, bool Invert)
			: base(UniqueKey)
		{
			this.Objective = Objective;
			this.Threshold = Threshold;
			this.Invert = Invert;
		}

		public TriggerObjective(ParseBlock Block)
			: base(Block.Name)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Objective = (Objective)attributes[(int)Attribute.OBJECTIVE];
			Threshold = (int)attributes[(int)Attribute.THRESHOLD];
			Invert = Parse.DefaultIfNull(attributes[(int)Attribute.INVERT], false);
		}

		public TriggerObjective(SerializationInputStream Stream)
			: this(
				Stream.ReadString(),
				(Objective)ObjectiveSerializer.Instance.Deserialize(Stream),
				Stream.ReadInt32(),
				Stream.ReadBoolean())
		{ }

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			ObjectiveSerializer.Instance.Serialize(Objective, Stream);
			Stream.Write(Threshold);
			Stream.Write(Invert);
		}

		public override int CalculateScore(Army ForArmy, Match Match)
		{
			bool t = Invert ? Objective.CalculateScore(ForArmy, Match) <= Threshold
									   : Objective.CalculateScore(ForArmy, Match) >= Threshold;
			_Score = t ? 1 : 0;
			return _Score;
		}
	}
}
