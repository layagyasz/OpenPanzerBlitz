using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class AttackFactorCalculation
	{
		public int Attack { get; private set; }
		public List<AttackFactorCalculationFactor> Factors { get; private set; }

		public AttackFactorCalculation(int Attack, IEnumerable<AttackFactorCalculationFactor> Factors)
		{
			this.Attack = Attack;
			this.Factors = Factors.ToList();
		}

		public AttackFactorCalculation(Unit Unit, AttackMethod AttackMethod, bool EnemyArmored, LineOfSight LineOfSight)
		{
			if (AttackMethod == AttackMethod.NORMAL_FIRE) GetNormalAttack(Unit, EnemyArmored, LineOfSight);
			else
			{
				if (Unit.CanAttack(AttackMethod, EnemyArmored, LineOfSight) != OrderInvalidReason.NONE)
				{
					Attack = 0;
					Factors = new List<AttackFactorCalculationFactor> { AttackFactorCalculationFactor.CANNOT_ATTACK };
				}
				Attack = Unit.Configuration.Attack;
				Factors = new List<AttackFactorCalculationFactor>();
			}
		}

		void GetNormalAttack(Unit Unit, bool EnemyArmored, LineOfSight LineOfSight)
		{
			if (Unit.CanAttack(AttackMethod.NORMAL_FIRE, EnemyArmored, LineOfSight) != OrderInvalidReason.NONE)
			{
				Attack = 0;
				Factors = new List<AttackFactorCalculationFactor> { AttackFactorCalculationFactor.CANNOT_ATTACK };
				return;
			}

			Factors = new List<AttackFactorCalculationFactor>();
			Attack = Unit.Configuration.Attack;

			if (EnemyArmored)
			{
				int HalfRange = Unit.Configuration.Range / 2;
				if (Unit.Configuration.WeaponClass == WeaponClass.HIGH_EXPLOSIVE
					|| Unit.Configuration.WeaponClass == WeaponClass.MORTAR)
				{
					if (LineOfSight.Range > HalfRange)
					{
						Attack /= 2;
						Factors.Add(AttackFactorCalculationFactor.ARMOR_RANGE);
					}
				}
				else if (Unit.Configuration.WeaponClass == WeaponClass.ANTI_ARMOR)
				{
					if (LineOfSight.Range <= HalfRange)
					{
						Attack *= 2;
						Factors.Add(AttackFactorCalculationFactor.ANTI_ARMOR_RANGE);
					}
				}
			}
			else if (Unit.Configuration.WeaponClass == WeaponClass.ANTI_ARMOR && !EnemyArmored)
			{
				Attack /= 2;
				Factors.Add(AttackFactorCalculationFactor.NOT_ARMORED);
			}

			if (LineOfSight.Initial.Rules.SubTieredElevation
				< LineOfSight.Final.Rules.SubTieredElevation)
			{
				Attack /= 2;
				Factors.Add(AttackFactorCalculationFactor.ELEVATION);
			}

			if (Unit.Configuration.Range < LineOfSight.Range && Unit.Configuration.CanDoubleRange)
			{
				Attack /= 2;
				Factors.Add(AttackFactorCalculationFactor.DOUBLE_RANGE);
			}
		}
	}
}
