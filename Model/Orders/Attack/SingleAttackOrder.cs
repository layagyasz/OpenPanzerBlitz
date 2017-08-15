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

		public abstract void Serialize(SerializationOutputStream Stream);

		public virtual NoSingleAttackReason Validate()
		{
			if (Defender == null) return NoSingleAttackReason.NONE;
			return Defender.CanBeAttackedBy(Attacker.Army);
		}

		public Army Army
		{
			get
			{
				return Attacker.Army;
			}
		}

		public abstract bool Execute(Random Random);
	}
}
