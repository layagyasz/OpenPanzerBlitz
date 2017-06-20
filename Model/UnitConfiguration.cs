﻿using System;
using System.Collections.Generic;

using Cardamom.Serialization;

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
			VERTICAL_SPLIT_IMAGE,

			CAN_DIRECT_FIRE,
			CAN_INDIRECT_FIRE,
			CAN_OVERRUN,
			CAN_CLOSE_ASSAULT,
			CAN_ANTI_AIRCRAFT,

			IS_ARMORED,

			TRUCK_MOVEMENT,
			IS_CARRIER,
			CAN_ONLY_CARRY_INFANTRY,
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
		public readonly bool VerticalSplitImage;

		public readonly bool CanDirectFire;
		public readonly bool CanIndirectFire;
		public readonly bool CanOverrun;
		public readonly bool CanCloseAssault;
		public readonly bool CanAntiAircraft;

		public readonly bool IsArmored;

		public readonly bool TruckMovement;

		public readonly bool IsCarrier;
		public readonly bool CanOnlyCarryInfantry;
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
			VerticalSplitImage = Parse.DefaultIfNull(attributes[(int)Attribute.VERTICAL_SPLIT_IMAGE], false);
			IsArmored = Parse.DefaultIfNull(attributes[(int)Attribute.IS_ARMORED], false);
			TruckMovement = Parse.DefaultIfNull(attributes[(int)Attribute.TRUCK_MOVEMENT], false);
			IsCarrier = Parse.DefaultIfNull(attributes[(int)Attribute.IS_CARRIER], false);
			CanOnlyCarryInfantry = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_ONLY_CARRY_INFANTRY], false);
			IsPassenger = Parse.DefaultIfNull(attributes[(int)Attribute.IS_PASSENGER],
											  UnitClass == UnitClass.INFANTRY
											  || UnitClass == UnitClass.ENGINEER
											  || UnitClass == UnitClass.PARATROOP
											  || UnitClass == UnitClass.COMMANDO
											  || UnitClass == UnitClass.TOWED_GUN);

			CanDirectFire = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_DIRECT_FIRE], true);
			CanIndirectFire = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_INDIRECT_FIRE], false);
			CanOverrun = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_OVERRUN], IsArmored && CanOnlyCarryInfantry);
			CanCloseAssault = Parse.DefaultIfNull(
				attributes[(int)Attribute.CAN_CLOSE_ASSAULT], UnitClass == UnitClass.INFANTRY
											  || UnitClass == UnitClass.ENGINEER
											  || UnitClass == UnitClass.PARATROOP
											  || UnitClass == UnitClass.COMMANDO);
			CanAntiAircraft = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_ANTI_AIRCRAFT], false);

		}

		public NoSingleAttackReason CanDirectFireAt(bool EnemyArmored, LineOfSight LineOfSight)
		{
			if (!CanDirectFire) return NoSingleAttackReason.NO_DIRECT_FIRE;
			if (LineOfSight.Range > Range) return NoSingleAttackReason.OUT_OF_RANGE;
			if (WeaponClass == WeaponClass.INFANTRY && EnemyArmored)
				return NoSingleAttackReason.NO_ARMOR_ATTACK;
			return NoSingleAttackReason.NONE;
		}

		public NoSingleAttackReason CanIndirectFireAt(LineOfSight LineOfSight)
		{
			if (!CanIndirectFire) return NoSingleAttackReason.NO_INDIRECT_FIRE;
			if (!LineOfSight.Final.CanIndirectFireAt) return NoSingleAttackReason.NO_INDIRECT_FIRE_SPOTTER;
			if (LineOfSight.Range > Range) return NoSingleAttackReason.OUT_OF_RANGE;
			return NoSingleAttackReason.NONE;
		}

		public AttackFactorCalculation GetAttack(AttackMethod AttackMethod, bool EnemyArmored, LineOfSight LineOfSight)
		{
			switch (AttackMethod)
			{
				case AttackMethod.NORMAL_FIRE:
					return GetNormalAttack(EnemyArmored, LineOfSight);
			}
			return null;
		}

		public AttackFactorCalculation GetNormalAttack(bool EnemyArmored, LineOfSight LineOfSight)
		{
			if (CanDirectFireAt(EnemyArmored, LineOfSight) != NoSingleAttackReason.NONE &&
				CanIndirectFireAt(LineOfSight) != NoSingleAttackReason.NONE)
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
