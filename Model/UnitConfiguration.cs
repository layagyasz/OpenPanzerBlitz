using System;
using System.Collections.Generic;

using Cardamom.Serialization;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class UnitConfiguration
	{
		private enum Attribute
		{
			NAME,
			UNIT_CLASS,
			WEAPON_CLASS,
			ATTACK,
			RANGE,
			DEFENSE,
			MOVEMENT,

			IMAGE_NAME,
			OVERRIDE_COLOR,

			CAN_ENGINEER,
			CAN_DIRECT_FIRE,
			CAN_INDIRECT_FIRE,
			CAN_OVERRUN,
			CAN_CLOSE_ASSAULT,
			CAN_ANTI_AIRCRAFT,

			IS_VEHICLE,
			IS_ARMORED,

			TRUCK_MOVEMENT,
			IS_CARRIER,
			CAN_ONLY_CARRY_INFANTRY,
			CAN_ONLY_OVERRUN_UNARMORED,
			IS_PASSENGER
		};

		public readonly string Name;
		public readonly UnitClass UnitClass;
		public readonly WeaponClass WeaponClass;

		public readonly byte Attack;
		public readonly byte Range;
		public readonly byte Defense;
		public readonly byte Movement;

		public readonly string ImageName;
		public readonly Color OverrideColor;

		public readonly bool CanEngineer;
		public readonly bool CanDirectFire;
		public readonly bool CanIndirectFire;
		public readonly bool CanOverrun;
		public readonly bool CanCloseAssault;
		public readonly bool CanAntiAircraft;

		public readonly bool IsVehicle;
		public readonly bool IsArmored;

		public readonly bool TruckMovement;

		public readonly bool IsCarrier;
		public readonly bool CanOnlyCarryInfantry;
		public readonly bool CanOnlyOverrunUnarmored;
		public readonly bool IsPassenger;

		public UnitConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			Name = (string)attributes[(int)Attribute.NAME];
			UnitClass = (UnitClass)attributes[(int)Attribute.UNIT_CLASS];
			WeaponClass = (WeaponClass)attributes[(int)Attribute.WEAPON_CLASS];
			Attack = (byte)attributes[(int)Attribute.ATTACK];
			Range = (byte)attributes[(int)Attribute.RANGE];
			Defense = (byte)attributes[(int)Attribute.DEFENSE];
			Movement = (byte)attributes[(int)Attribute.MOVEMENT];
			ImageName = (string)attributes[(int)Attribute.IMAGE_NAME];
			OverrideColor = Parse.DefaultIfNull(attributes[(int)Attribute.OVERRIDE_COLOR], Color.Black);
			IsVehicle = Parse.DefaultIfNull(attributes[(int)Attribute.IS_VEHICLE],
											UnitClass == UnitClass.AMPHIBIOUS_VEHICLE
											|| UnitClass == UnitClass.ASSAULT_GUN
											|| UnitClass == UnitClass.ENGINEER_VEHICLE
											|| UnitClass == UnitClass.FLAME_TANK
											|| UnitClass == UnitClass.RECONNAISSANCE_VEHICLE
											|| UnitClass == UnitClass.SELF_PROPELLED_ARTILLERY
											|| UnitClass == UnitClass.TANK
											|| UnitClass == UnitClass.TRANSPORT);
			IsArmored = Parse.DefaultIfNull(
				attributes[(int)Attribute.IS_ARMORED], IsVehicle && UnitClass != UnitClass.TRANSPORT);
			TruckMovement = Parse.DefaultIfNull(attributes[(int)Attribute.TRUCK_MOVEMENT], false);
			IsCarrier = Parse.DefaultIfNull(
				attributes[(int)Attribute.IS_CARRIER], IsArmored && UnitClass == UnitClass.TRANSPORT);
			CanOnlyCarryInfantry = Parse.DefaultIfNull(
				attributes[(int)Attribute.CAN_ONLY_CARRY_INFANTRY], IsArmored && UnitClass != UnitClass.TRANSPORT);
			IsPassenger = Parse.DefaultIfNull(attributes[(int)Attribute.IS_PASSENGER],
											  UnitClass == UnitClass.INFANTRY
											  || UnitClass == UnitClass.PARATROOP
											  || UnitClass == UnitClass.COMMANDO
											  || UnitClass == UnitClass.TOWED_GUN);

			CanEngineer = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_ENGINEER], false);
			CanDirectFire = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_DIRECT_FIRE], true);
			CanIndirectFire = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_INDIRECT_FIRE], false);
			CanOverrun = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_OVERRUN], IsVehicle && IsArmored);
			CanOnlyOverrunUnarmored = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_ONLY_OVERRUN_UNARMORED], false);
			CanCloseAssault = Parse.DefaultIfNull(
				attributes[(int)Attribute.CAN_CLOSE_ASSAULT], UnitClass == UnitClass.INFANTRY
											  || UnitClass == UnitClass.PARATROOP
											  || UnitClass == UnitClass.COMMANDO);
			CanAntiAircraft = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_ANTI_AIRCRAFT], false);

		}

		private NoSingleAttackReason CanDirectFireAt(bool EnemyArmored, LineOfSight LineOfSight)
		{
			if (!CanDirectFire) return NoSingleAttackReason.UNABLE;
			if (LineOfSight.Range > Range) return NoSingleAttackReason.OUT_OF_RANGE;
			if (WeaponClass == WeaponClass.INFANTRY && EnemyArmored)
				return NoSingleAttackReason.NO_ARMOR_ATTACK;
			return NoSingleAttackReason.NONE;
		}

		private NoSingleAttackReason CanIndirectFireAt(LineOfSight LineOfSight)
		{
			if (!CanIndirectFire) return NoSingleAttackReason.UNABLE;
			if (!LineOfSight.Final.CanIndirectFireAt) return NoSingleAttackReason.NO_INDIRECT_FIRE_SPOTTER;
			if (LineOfSight.Range > Range) return NoSingleAttackReason.OUT_OF_RANGE;
			return NoSingleAttackReason.NONE;
		}

		public NoSingleAttackReason CanAttack(AttackMethod AttackMethod, bool EnemyArmored, LineOfSight LineOfSight)
		{
			switch (AttackMethod)
			{
				case AttackMethod.NORMAL_FIRE:
					if (CanDirectFireAt(EnemyArmored, LineOfSight) == NoSingleAttackReason.NONE)
						return NoSingleAttackReason.NONE;
					return CanIndirectFireAt(LineOfSight);
				case AttackMethod.OVERRUN:
					if (!CanOverrun) return NoSingleAttackReason.UNABLE;
					if (CanOnlyOverrunUnarmored && EnemyArmored) return NoSingleAttackReason.NO_ARMOR_ATTACK;
					return NoSingleAttackReason.NONE;
			}
			return NoSingleAttackReason.NONE;
		}

		public AttackFactorCalculation GetAttack(AttackMethod AttackMethod, bool EnemyArmored, LineOfSight LineOfSight)
		{
			switch (AttackMethod)
			{
				case AttackMethod.NORMAL_FIRE:
					return GetNormalAttack(EnemyArmored, LineOfSight);
				case AttackMethod.OVERRUN:
					if (CanAttack(AttackMethod, EnemyArmored, LineOfSight) != NoSingleAttackReason.NONE)
						return new AttackFactorCalculation(
							0, new List<AttackFactorCalculationFactor>()
							{
								AttackFactorCalculationFactor.CANNOT_ATTACK
							});
					return new AttackFactorCalculation(Attack, new List<AttackFactorCalculationFactor>());
			}
			return null;
		}

		private AttackFactorCalculation GetNormalAttack(bool EnemyArmored, LineOfSight LineOfSight)
		{
			if (CanAttack(AttackMethod.NORMAL_FIRE, EnemyArmored, LineOfSight) != NoSingleAttackReason.NONE)
			{
				return new AttackFactorCalculation(
					0, new List<AttackFactorCalculationFactor> { AttackFactorCalculationFactor.CANNOT_ATTACK });
			}

			List<AttackFactorCalculationFactor> factors = new List<AttackFactorCalculationFactor>();
			int attack = Attack;
			if (LineOfSight.Initial.Elevation < LineOfSight.Final.Elevation)
			{
				attack /= 2;
				factors.Add(AttackFactorCalculationFactor.ELEVATION);
			}

			if (EnemyArmored)
			{
				int HalfRange = Range / 2;
				if (WeaponClass == WeaponClass.HIGH_EXPLOSIVE || WeaponClass == WeaponClass.MORTAR)
				{
					if (LineOfSight.Range > HalfRange)
					{
						attack /= 2;
						factors.Add(AttackFactorCalculationFactor.ARMOR_RANGE);
					}
				}
				else if (WeaponClass == WeaponClass.ANTI_ARMOR)
				{
					if (LineOfSight.Range <= HalfRange)
					{
						attack *= 2;
						factors.Add(AttackFactorCalculationFactor.ANTI_ARMOR_RANGE);
					}
				}
			}
			else if (WeaponClass == WeaponClass.ANTI_ARMOR && !EnemyArmored)
			{
				attack /= 2;
				factors.Add(AttackFactorCalculationFactor.NOT_ARMORED);
			}

			return new AttackFactorCalculation(attack, factors);
		}
	}
}
