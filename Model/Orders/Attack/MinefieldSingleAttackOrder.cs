using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class MinefieldSingleAttackOrder : SingleAttackOrder
	{
		Unit _Attacker;
		Unit _Defender;

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

		public MinefieldSingleAttackOrder(Unit Attacker, Unit Defender)
		{
			_Attacker = Attacker;
			_Defender = Defender;
		}

		public void SetTreatStackAsArmored(bool TreatStackAsArmored)
		{
			// Armored doesn't matter.
		}

		public AttackFactorCalculation GetAttack()
		{
			return new AttackFactorCalculation(
				_Defender.Configuration.Defense * _Attacker.Configuration.Attack,
				new List<AttackFactorCalculationFactor>() { AttackFactorCalculationFactor.MINEFIELD });
		}

		public NoSingleAttackReason Validate()
		{
			if (_Attacker.CanAttack(AttackMethod.MINEFIELD) != NoSingleAttackReason.NONE)
				return NoSingleAttackReason.UNABLE;
			if (_Attacker.Position != _Defender.Position) return NoSingleAttackReason.OUT_OF_RANGE;
			return NoSingleAttackReason.NONE;
		}

		public bool Execute(Random Random)
		{
			return Validate() == NoSingleAttackReason.NONE;
		}
	}
}
