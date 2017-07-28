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
			if (Defender != null && !Defender.CanBeAttackedBy(Attacker.Army)) return NoSingleAttackReason.TEAM;
			return NoSingleAttackReason.NONE;
		}

		public abstract bool Execute(Random Random);
	}
}
