using System;
using System.Collections.Generic;

using Cardamom.Serialization;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class UnitConfiguration
	{
		enum Attribute
		{
			NAME,
			UNIT_CLASS,
			WEAPON_CLASS,
			ATTACK,
			RANGE,
			DEFENSE,
			MOVEMENT,

			CAN_DIRECT_FIRE,
			CAN_INDIRECT_FIRE,
			CAN_OVERRUN,
			CAN_CLOSE_ASSAULT,
			CAN_ANTI_AIRCRAFT,
			CAN_DOUBLE_RANGE,

			IS_VEHICLE,
			IS_ARMORED,
			LEAVES_WRECK_WHEN_DESTROYED,
			IS_ENGINEER,
			IS_PARATROOP,
			IS_COMMANDO,

			MOVEMENT_RULES,

			IS_CARRIER,
			CAN_ONLY_CARRY_INFANTRY,
			CAN_ONLY_CARRY_LIGHT,
			CAN_ONLY_OVERRUN_UNARMORED,
			CAN_ONLY_SUPPORT_CLOSE_ASSAULT,
			IS_PASSENGER,
			IS_LIGHT_PASSENGER,
			IS_OVERSIZED_PASSENGER,
			CANNOT_USE_ROAD_MOVEMENT_WITH_OVERSIZED_PASSENGER,
			OVERSIZED_PASSENGER_MOVEMENT_MULTIPLIER,

			CAN_SPOT_INDIRECT_FIRE,
			DISMOUNT_AS,
			CAN_REMOUNT
		};

		public readonly string UniqueKey;
		public readonly string Name;
		public readonly UnitClass UnitClass;
		public readonly WeaponClass WeaponClass;

		public readonly byte Attack;
		public readonly byte Range;
		public readonly byte Defense;
		public readonly byte Movement;

		public readonly bool CanDirectFire;
		public readonly bool CanIndirectFire;
		public readonly bool CanOverrun;
		public readonly bool CanCloseAssault;
		public readonly bool CanAntiAircraft;
		public readonly bool CanDoubleRange;

		public readonly bool IsVehicle;
		public readonly bool IsArmored;
		public readonly bool LeavesWreckWhenDestroyed;
		public readonly bool IsEngineer;
		public readonly bool IsParatroop;
		public readonly bool IsCommando;

		public readonly UnitMovementRules MovementRules;

		public readonly bool IsCarrier;
		public readonly bool CanOnlyCarryInfantry;
		public readonly bool CanOnlyCarryLight;
		public readonly bool CanOnlyOverrunUnarmored;
		public readonly bool CanOnlySupportCloseAssault;
		public readonly bool IsPassenger;
		public readonly bool IsLightPassenger;
		public readonly bool IsOversizedPassenger;
		public readonly bool CannotUseRoadMovementWithOversizedPassenger;
		public readonly float OversizedPassengerMovementMultiplier;

		public readonly bool CanSpotIndirectFire;

		public readonly UnitConfiguration DismountAs;
		public readonly bool CanRemount;

		public IEnumerable<UnitConfiguration> RepresentedConfigurations
		{
			get
			{
				yield return this;
				if (DismountAs != null) yield return DismountAs;
			}
		}

		public UnitConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UniqueKey = Block.Name;
			Name = (string)attributes[(int)Attribute.NAME];
			UnitClass = (UnitClass)attributes[(int)Attribute.UNIT_CLASS];
			WeaponClass = (WeaponClass)attributes[(int)Attribute.WEAPON_CLASS];
			Attack = (byte)attributes[(int)Attribute.ATTACK];
			Range = (byte)attributes[(int)Attribute.RANGE];
			Defense = (byte)attributes[(int)Attribute.DEFENSE];
			Movement = (byte)attributes[(int)Attribute.MOVEMENT];
			IsVehicle = Parse.DefaultIfNull(attributes[(int)Attribute.IS_VEHICLE],
											UnitClass == UnitClass.AMPHIBIOUS_VEHICLE
											|| UnitClass == UnitClass.ASSAULT_GUN
											|| UnitClass == UnitClass.ENGINEER_VEHICLE
											|| UnitClass == UnitClass.FLAME_TANK
											|| UnitClass == UnitClass.RECONNAISSANCE_VEHICLE
											|| UnitClass == UnitClass.SELF_PROPELLED_ARTILLERY
											|| UnitClass == UnitClass.TANK
											|| UnitClass == UnitClass.TRANSPORT
											|| UnitClass == UnitClass.WRECKAGE);
			IsArmored = Parse.DefaultIfNull(
				attributes[(int)Attribute.IS_ARMORED],
				(IsVehicle && UnitClass != UnitClass.TRANSPORT) || UnitClass == UnitClass.FORT);
			LeavesWreckWhenDestroyed = Parse.DefaultIfNull(
				attributes[(int)Attribute.LEAVES_WRECK_WHEN_DESTROYED], IsArmored && IsVehicle);
			IsParatroop = Parse.DefaultIfNull(attributes[(int)Attribute.IS_PARATROOP], false);
			IsCommando = Parse.DefaultIfNull(attributes[(int)Attribute.IS_COMMANDO], false);

			MovementRules = Parse.DefaultIfNull(
				attributes[(int)Attribute.MOVEMENT_RULES],
				IsVehicle
					? Block.Get<UnitMovementRules>("unit-movement-rules.default-vehicle")
					: Block.Get<UnitMovementRules>("unit-movement-rules.default-non-vehicle"));

			IsCarrier = Parse.DefaultIfNull(
				attributes[(int)Attribute.IS_CARRIER], IsVehicle || UnitClass == UnitClass.TRANSPORT);
			CanOnlyCarryInfantry = Parse.DefaultIfNull(
				attributes[(int)Attribute.CAN_ONLY_CARRY_INFANTRY], IsCarrier && UnitClass != UnitClass.TRANSPORT);
			CanOnlyCarryLight = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_ONLY_CARRY_LIGHT], false);
			IsPassenger = Parse.DefaultIfNull(attributes[(int)Attribute.IS_PASSENGER],
											  UnitClass == UnitClass.INFANTRY
											  || UnitClass == UnitClass.COMMAND_POST
											  || UnitClass == UnitClass.TOWED_GUN);
			IsLightPassenger = Parse.DefaultIfNull(
				attributes[(int)Attribute.IS_LIGHT_PASSENGER],
				IsPassenger && (UnitClass == UnitClass.INFANTRY || UnitClass == UnitClass.COMMAND_POST));
			IsOversizedPassenger = Parse.DefaultIfNull(
				attributes[(int)Attribute.IS_OVERSIZED_PASSENGER], false);
			CannotUseRoadMovementWithOversizedPassenger = Parse.DefaultIfNull(
				attributes[(int)Attribute.CANNOT_USE_ROAD_MOVEMENT_WITH_OVERSIZED_PASSENGER], CanOnlyCarryInfantry);
			OversizedPassengerMovementMultiplier = Parse.DefaultIfNull(
				attributes[(int)Attribute.OVERSIZED_PASSENGER_MOVEMENT_MULTIPLIER], 1f);

			IsEngineer = Parse.DefaultIfNull(attributes[(int)Attribute.IS_ENGINEER], false);
			CanDirectFire = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_DIRECT_FIRE], Attack > 0);
			CanIndirectFire = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_INDIRECT_FIRE],
												  UnitClass == UnitClass.SELF_PROPELLED_ARTILLERY);
			CanOverrun = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_OVERRUN],
											 IsVehicle && IsArmored && UnitClass != UnitClass.SELF_PROPELLED_ARTILLERY);
			CanOnlyOverrunUnarmored = Parse.DefaultIfNull(
				attributes[(int)Attribute.CAN_ONLY_OVERRUN_UNARMORED],
				CanOverrun && WeaponClass == WeaponClass.INFANTRY);
			CanCloseAssault = Parse.DefaultIfNull(
				attributes[(int)Attribute.CAN_CLOSE_ASSAULT],
				UnitClass == UnitClass.INFANTRY || UnitClass == UnitClass.CAVALRY);
			CanOnlySupportCloseAssault = Parse.DefaultIfNull(
				attributes[(int)Attribute.CAN_ONLY_SUPPORT_CLOSE_ASSAULT], false);
			CanAntiAircraft = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_ANTI_AIRCRAFT], false);
			CanDoubleRange = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_DOUBLE_RANGE], false);

			CanSpotIndirectFire = Parse.DefaultIfNull(
				attributes[(int)Attribute.CAN_SPOT_INDIRECT_FIRE], UnitClass == UnitClass.COMMAND_POST);

			DismountAs = (UnitConfiguration)attributes[(int)Attribute.DISMOUNT_AS];
			CanRemount = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_REMOUNT], DismountAs != null);
		}

		public OrderInvalidReason CanLoad(UnitConfiguration UnitConfiguration)
		{
			if (!IsCarrier
				|| !UnitConfiguration.IsPassenger
				|| (CanOnlyCarryInfantry && UnitConfiguration.UnitClass != UnitClass.INFANTRY)
				|| (CanOnlyCarryLight && !UnitConfiguration.IsLightPassenger))
				return OrderInvalidReason.UNIT_NO_CARRY;
			return OrderInvalidReason.NONE;
		}

		public int GetStackSize()
		{
			if (UnitClass == UnitClass.BLOCK
				|| UnitClass == UnitClass.MINEFIELD
				|| UnitClass == UnitClass.FORT
				|| UnitClass == UnitClass.BRIDGE)
				return 0;
			return 1;
		}

		public bool IsStackUnique()
		{
			return UnitClass == UnitClass.FORT
										 || UnitClass == UnitClass.BLOCK
										 || UnitClass == UnitClass.MINEFIELD
										 || UnitClass == UnitClass.BRIDGE;
		}

		public BlockType GetBlockType()
		{
			if (UnitClass == UnitClass.FORT) return BlockType.NONE;
			if (UnitClass == UnitClass.MINEFIELD) return BlockType.SOFT_BLOCK;
			if (UnitClass == UnitClass.BLOCK) return BlockType.HARD_BLOCK;
			return BlockType.STANDARD;
		}

		public bool IsNeutral()
		{
			return UnitClass == UnitClass.MINEFIELD
										 || UnitClass == UnitClass.BLOCK
										 || UnitClass == UnitClass.WRECKAGE;
		}

		public byte GetAdjustedRange()
		{
			return (byte)(CanDoubleRange ? 2 * Range : Range);
		}

		public OrderInvalidReason CanDirectFireAt(bool EnemyArmored, LineOfSight LineOfSight)
		{
			if (!CanDirectFire) return OrderInvalidReason.UNIT_NO_ATTACK;
			if (LineOfSight.Range > GetAdjustedRange()) return OrderInvalidReason.TARGET_OUT_OF_RANGE;
			if (WeaponClass == WeaponClass.INFANTRY && EnemyArmored)
				return OrderInvalidReason.TARGET_ARMORED;
			return OrderInvalidReason.NONE;
		}

		public OrderInvalidReason CanIndirectFireAt(LineOfSight LineOfSight)
		{
			if (LineOfSight.Range > GetAdjustedRange()) return OrderInvalidReason.TARGET_OUT_OF_RANGE;
			if (!CanIndirectFire) return OrderInvalidReason.UNIT_NO_ATTACK;
			return OrderInvalidReason.NONE;
		}

		public OrderInvalidReason CanOverrunAt(bool EnemyArmored)
		{
			if (CanOnlyOverrunUnarmored && EnemyArmored) return OrderInvalidReason.TARGET_ARMORED;
			return OrderInvalidReason.NONE;
		}

		public OrderInvalidReason CanCloseAssaultAt(LineOfSight LineOfSight)
		{
			if (!CanCloseAssault) return OrderInvalidReason.UNIT_NO_ATTACK;
			if (LineOfSight.Range > 1) return OrderInvalidReason.TARGET_OUT_OF_RANGE;
			return OrderInvalidReason.NONE;
		}

		public OrderInvalidReason CanAttack(AttackMethod AttackMethod)
		{
			switch (AttackMethod)
			{
				case AttackMethod.OVERRUN:
					return CanOverrun ? OrderInvalidReason.NONE : OrderInvalidReason.UNIT_NO_ATTACK;
				case AttackMethod.NORMAL_FIRE:
					return CanDirectFire || CanIndirectFire
						? OrderInvalidReason.NONE : OrderInvalidReason.UNIT_NO_ATTACK;
				case AttackMethod.CLOSE_ASSAULT:
					return CanCloseAssault ? OrderInvalidReason.NONE : OrderInvalidReason.UNIT_NO_ATTACK;
				default: return OrderInvalidReason.NONE;
			}
		}

		public int GetRange(AttackMethod AttackMethod)
		{
			switch (AttackMethod)
			{
				case AttackMethod.OVERRUN: return 0;
				case AttackMethod.NORMAL_FIRE: return GetAdjustedRange();
				case AttackMethod.CLOSE_ASSAULT: return CanCloseAssault ? 1 : 0;
			}
			// Should not end up here.
			return 0;
		}

		public float GetMaxMovement(Environment Environment)
		{
			if (MovementRules.IgnoresEnvironmentMovement) return Movement;
			if (Movement == 0) return 0;
			return Math.Max(1, Environment.MovementMultiplier * Movement);
		}

		public float GetPointValue()
		{
			if (DismountAs == null) return GetPointValueInternal();
			return Math.Max(GetPointValueInternal(), DismountAs.GetPointValue());
		}

		float GetPointValueInternal()
		{
			switch (UnitClass)
			{
				case UnitClass.TANK:
					if (WeaponClass == WeaponClass.INFANTRY) return Defense + Movement;
					else return Attack + Range + Defense + Movement;
				case UnitClass.ASSAULT_GUN:
					if (CanAntiAircraft)
					{
						if (WeaponClass == WeaponClass.INFANTRY) return .5f * Attack + .5f * Range + Defense + Movement;
						else return 1.5f * Attack + .5f * Range + Defense + Movement;
					}
					else return Attack + Math.Min((int)Defense, 6) + Defense + Movement;
				case UnitClass.FLAME_TANK:
					return .5f * Attack + Range + Defense + Movement;
				case UnitClass.SELF_PROPELLED_ARTILLERY:
					if (Range > 16) return Attack + .25f * Range + Defense + Movement;
					else return Attack + 4 + Defense + Movement;
				case UnitClass.RECONNAISSANCE_VEHICLE:
					if (CanSpotIndirectFire) return 5 + Defense + Movement;
					if (CanAntiAircraft)
					{
						if (WeaponClass == WeaponClass.INFANTRY) return .5f * Attack + .5f * Range + Defense + Movement;
						else return 1.5f * Attack + .5f * Range + Defense + Movement;
					}
					else
					{
						if (WeaponClass == WeaponClass.INFANTRY) return Defense + Movement;
						else if (WeaponClass == WeaponClass.ANTI_ARMOR) return Attack + Range + Defense + Movement;
						else return Attack + Math.Min((int)Range, 6) + Defense + Movement;
					}
				case UnitClass.TRANSPORT:
					if (IsVehicle)
					{
						if (IsArmored) return Attack + Range + Defense + Movement;
						else return Attack + Range + Defense + .5f * Movement;
					}
					else return Attack + Range + Defense + Movement;
				case UnitClass.TOWED_GUN:
					if (CanIndirectFire && WeaponClass == WeaponClass.HIGH_EXPLOSIVE)
						return .5f * Attack + .25f * Range + Defense + Movement;
					else if (CanAntiAircraft)
					{
						if (WeaponClass == WeaponClass.INFANTRY) return .5f * Attack + .5f + Range + Defense + Movement;
						else return Attack + .5f * Range + Defense + Movement;
					}
					else if (WeaponClass == WeaponClass.HIGH_EXPLOSIVE)
						return .5f * Attack + Math.Min((int)Range, 6) + Defense + Movement;
					else return .5f * Attack + .5f * Range + Defense + Movement;
				case UnitClass.INFANTRY:
					if (IsEngineer) return 2 * Attack + 1 + Defense + Movement;
					else if (IsCommando) return 2 * Attack + 1 + Defense + 2 * Movement;
					else return Attack + 1 + Defense + Movement;
				case UnitClass.CAVALRY:
					return Attack + 1 + Defense + 1;
				case UnitClass.COMMAND_POST:
					return 5;
				case UnitClass.AMPHIBIOUS_VEHICLE:
					if (IsCarrier || WeaponClass == WeaponClass.INFANTRY) return Defense + 1.5f * Movement;
					else if (WeaponClass == WeaponClass.HIGH_EXPLOSIVE)
						return Attack + Math.Min((int)Range, 6) + Defense + 1.5f * Movement;
					else return Attack + Range + Defense + 1.5f * Movement;
				case UnitClass.ENGINEER_VEHICLE:
					return Attack + Range + Defense + 1.5f * Movement;
				case UnitClass.BRIDGE:
					return Defense;
				case UnitClass.FORT:
					return 10 + .5f * Defense;
				case UnitClass.BLOCK:
					return 12;
				case UnitClass.MINEFIELD:
					return 10 * Attack + 15;
				default:
					return Attack + Range + Defense + Movement;
			}
		}

		public override string ToString()
		{
			return string.Format("[UnitConfiguration: Name={0}]", Name);
		}
	}
}
