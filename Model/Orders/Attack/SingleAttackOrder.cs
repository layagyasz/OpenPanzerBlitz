using System;
namespace PanzerBlitz
{
	public abstract class SingleAttackOrder : Order
	{
		public abstract Unit Attacker { get; }
		public abstract Unit Defender { get; }
		public abstract void SetTreatStackAsArmored(bool TreatStackAsArmored);
		public abstract AttackFactorCalculation GetAttack();

		public virtual NoSingleAttackReason Validate()
		{
			if (Defender == null) return NoSingleAttackReason.ILLEGAL;
			return Defender.CanBeAttackedBy(Attacker.Army);
		}

		public abstract bool Execute(Random Random);
	}
}
