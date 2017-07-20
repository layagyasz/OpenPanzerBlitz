using System;
namespace PanzerBlitz
{
	public interface SingleAttackOrder : Order
	{
		Unit Attacker { get; }
		Unit Defender { get; }
		void SetTreatStackAsArmored(bool TreatStackAsArmored);
		AttackFactorCalculation GetAttack();
		NoSingleAttackReason Validate();
	}
}
