using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public abstract class SingleAttackOrder : Order
	{
		public abstract Unit Attacker { get; }
		public abstract Unit Defender { get; }
		public abstract void SetTreatStackAsArmored(bool TreatStackAsArmored);
		public abstract AttackFactorCalculation GetAttack();

		public Army Army
		{
			get
			{
				return Attacker.Army;
			}		}

		public abstract void Serialize(SerializationOutputStream Stream);

		public virtual OrderInvalidReason Validate()
		{
			return OrderInvalidReason.NONE;
		}

		public abstract bool MatchesTurnComponent(TurnComponent TurnComponent);
		public abstract OrderStatus Execute(Random Random);
	}
}
