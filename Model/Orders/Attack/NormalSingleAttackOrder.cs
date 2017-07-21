using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class NormalSingleAttackOrder : SingleAttackOrder
	{
		public readonly AttackMethod AttackMethod;
		public readonly LineOfSight LineOfSight;

		Unit _Attacker;
		Unit _Defender;
		bool _TreatStackAsArmored;

		public Unit Attacker
		{
			get
			{
				return _Attacker;
			}
		}

		public Unit Defender
		{
			get
			{
				return _Defender;
			}
		}

		public NormalSingleAttackOrder(Unit Attacker, Unit Defender, AttackMethod AttackMethod)
		{
			_Attacker = Attacker;
			_Defender = Defender;
			this.AttackMethod = AttackMethod;
			this.LineOfSight = Attacker.GetLineOfSight(Defender.Position);
		}

		public void SetTreatStackAsArmored(bool TreatStackAsArmored)
		{
			_TreatStackAsArmored = TreatStackAsArmored;
		}

		public AttackFactorCalculation GetAttack()
		{
			if (Validate() == NoSingleAttackReason.NONE)
				return _Attacker.Configuration.GetAttack(AttackMethod, _TreatStackAsArmored, LineOfSight);
			return new AttackFactorCalculation(
				0, new List<AttackFactorCalculationFactor>() { AttackFactorCalculationFactor.CANNOT_ATTACK });
		}

		public NoSingleAttackReason Validate()
		{
			if (LineOfSight.Validate() != NoLineOfSightReason.NONE) return NoSingleAttackReason.NO_LOS;
			NoSingleAttackReason r = _Attacker.CanAttack(AttackMethod, _TreatStackAsArmored, LineOfSight);
			if (r != NoSingleAttackReason.NONE) return r;

			return NoSingleAttackReason.NONE;
		}

		public bool Execute(Random Random)
		{
			if (Validate() == NoSingleAttackReason.NONE)
			{
				_Attacker.Fire();
				return true;
			}
			return false;
		}
	}
}
