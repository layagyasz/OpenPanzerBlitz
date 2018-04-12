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

		public AttackFactorCalculation(
			Unit Unit,
			AttackMethod AttackMethod,
			bool EnemyArmored,
			LineOfSight LineOfSight,
			bool UseSecondaryWeapon)
		{
			switch (AttackMethod)
			{
				case AttackMethod.NORMAL_FIRE:
					GetNormalAttack(Unit, EnemyArmored, LineOfSight, UseSecondaryWeapon);
					break;
				case AttackMethod.AIR:
					GetAirAttack(Unit, EnemyArmored, UseSecondaryWeapon);
					break;
				case AttackMethod.ANTI_AIRCRAFT:
					GetAntiAirAttack(Unit, LineOfSight, UseSecondaryWeapon);
					break;
				default:
					if (Unit.CanAttack(
						AttackMethod, EnemyArmored, LineOfSight, UseSecondaryWeapon) != OrderInvalidReason.NONE)
					{
						Attack = 0;
						Factors = new List<AttackFactorCalculationFactor>
						{
							AttackFactorCalculationFactor.CANNOT_ATTACK
						};
					}
					Attack = Unit.Configuration.GetWeapon(UseSecondaryWeapon).Attack;
					Factors = new List<AttackFactorCalculationFactor>();
					break;
			}
		}

		void GetAntiAirAttack(Unit Unit, LineOfSight LineOfSight, bool UseSecondaryWeapon)
		{
			var weapon = Unit.Configuration.GetWeapon(UseSecondaryWeapon);
			Factors = new List<AttackFactorCalculationFactor>();
			Attack = weapon.Attack;
			if (LineOfSight.Range < weapon.Range / 2)
			{
				Attack *= 2;
				Factors.Add(AttackFactorCalculationFactor.ANTI_AIR_RANGE);
			}
		}

		void GetAirAttack(Unit Unit, bool EnemyArmored, bool UseSecondaryWeapon)
		{
			var weapon = Unit.Configuration.GetWeapon(UseSecondaryWeapon);
			Factors = new List<AttackFactorCalculationFactor>();
			Attack = weapon.Attack;
			if (EnemyArmored)
			{
				if (weapon.WeaponClass == WeaponClass.HIGH_EXPLOSIVE || weapon.WeaponClass == WeaponClass.MORTAR)
				{
					Attack /= 2;
					Factors.Add(AttackFactorCalculationFactor.ARMOR_RANGE);
				}
			}
			else
			{
				if (weapon.WeaponClass == WeaponClass.ANTI_ARMOR)
				{
					Attack /= 2;
					Factors.Add(AttackFactorCalculationFactor.NOT_ARMORED);
				}
			}
		}

		void GetNormalAttack(Unit Unit, bool EnemyArmored, LineOfSight LineOfSight, bool UseSecondaryWeapon)
		{
			if (Unit.CanAttack(
				AttackMethod.NORMAL_FIRE, EnemyArmored, LineOfSight, UseSecondaryWeapon) != OrderInvalidReason.NONE)
			{
				Attack = 0;
				Factors = new List<AttackFactorCalculationFactor> { AttackFactorCalculationFactor.CANNOT_ATTACK };
				return;
			}

			var weapon = Unit.Configuration.GetWeapon(UseSecondaryWeapon);
			Factors = new List<AttackFactorCalculationFactor>();
			Attack = weapon.Attack;

			if (EnemyArmored)
			{
				int HalfRange = weapon.Range / 2;
				if (weapon.WeaponClass == WeaponClass.HIGH_EXPLOSIVE
					|| weapon.WeaponClass == WeaponClass.MORTAR)
				{
					if (LineOfSight.Range > HalfRange)
					{
						Attack /= 2;
						Factors.Add(AttackFactorCalculationFactor.ARMOR_RANGE);
					}
				}
				else if (weapon.WeaponClass == WeaponClass.ANTI_ARMOR)
				{
					if (LineOfSight.Range <= HalfRange)
					{
						Attack *= 2;
						Factors.Add(AttackFactorCalculationFactor.ANTI_ARMOR_RANGE);
					}
				}
			}
			else if (weapon.WeaponClass == WeaponClass.ANTI_ARMOR && !EnemyArmored)
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

			if (weapon.Range < LineOfSight.Range && weapon.CanDoubleRange)
			{
				Attack /= 2;
				Factors.Add(AttackFactorCalculationFactor.DOUBLE_RANGE);
			}
		}
	}
}
