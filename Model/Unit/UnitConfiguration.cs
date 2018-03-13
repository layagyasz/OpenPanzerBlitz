using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitConfiguration : Serializable
	{
		enum Attribute
		{
			NAME,
			UNIT_CLASS,

			PRIMARY_WEAPON,
			SECONDARY_WEAPON,
			WEAPON_CLASS,
			ATTACK,
			RANGE,

			DEFENSE,
			MOVEMENT,

			CAN_DIRECT_FIRE,
			CAN_INDIRECT_FIRE,
			CAN_OVERRUN,
			CAN_CLOSE_ASSAULT,
			CAN_AIR_ATTACK,
			CAN_ANTI_AIRCRAFT,
			CAN_DOUBLE_RANGE,
			CAN_CLEAR_MINES,
			CAN_PLACE_MINES,
			CAN_PLACE_BRIDGES,
			INNATELY_CLEARS_MINES,
			IMMUNE_TO_MINES,

			IS_VEHICLE,
			IS_ARMORED,
			LEAVES_WRECK_WHEN_DESTROYED,
			IS_ENGINEER,
			IS_PARATROOP,
			IS_COMMANDO,
			HAS_LOW_PROFILE,

			MOVEMENT_RULES,

			IS_CARRIER,
			CAN_ONLY_CARRY_INFANTRY,
			CAN_ONLY_CARRY_LIGHT,
			CAN_CARRY_IN_WATER,
			CAN_ONLY_OVERRUN_UNARMORED,
			CAN_ONLY_SUPPORT_CLOSE_ASSAULT,
			IS_PASSENGER,
			IS_LIGHT_PASSENGER,
			IS_OVERSIZED_PASSENGER,
			CANNOT_USE_ROAD_MOVEMENT_WITH_OVERSIZED_PASSENGER,
			OVERSIZED_PASSENGER_MOVEMENT_MULTIPLIER,
			WATER_DIE_MODIFIER,

			CAN_SPOT,
			SPOT_RANGE,
			DISMOUNT_AS,
			CAN_REMOUNT,

			CAN_SUPPORT_ARMORED
		};

		public readonly string UniqueKey;
		public readonly string Name;
		public readonly UnitClass UnitClass;

		public readonly Weapon PrimaryWeapon;
		public readonly Weapon SecondaryWeapon;
		public readonly byte Defense;
		public readonly byte Movement;

		public readonly bool CanDirectFire;
		public readonly bool CanIndirectFire;
		public readonly bool CanOverrun;
		public readonly bool CanCloseAssault;
		public readonly bool CanAirAttack;
		public readonly bool CanAntiAircraft;
		public readonly bool CanClearMines;
		public readonly bool CanPlaceMines;
		public readonly bool CanPlaceBridges;
		public readonly bool InnatelyClearsMines;
		public readonly bool ImmuneToMines;

		public readonly bool IsVehicle;
		public readonly bool IsArmored;
		public readonly bool LeavesWreckWhenDestroyed;
		public readonly bool IsEngineer;
		public readonly bool IsParatroop;
		public readonly bool IsCommando;
		public readonly bool HasLowProfile;

		public readonly UnitMovementRules MovementRules;

		public readonly bool IsCarrier;

		public readonly bool CanOnlyCarryInfantry;
		public readonly bool CanOnlyCarryLight;
		public readonly bool CanCarryInWater;
		public readonly bool CanOnlyOverrunUnarmored;
		public readonly bool CanOnlySupportCloseAssault;
		public readonly bool IsPassenger;
		public readonly bool IsLightPassenger;
		public readonly bool IsOversizedPassenger;
		public readonly bool CannotUseRoadMovementWithOversizedPassenger;
		public readonly float OversizedPassengerMovementMultiplier;
		public readonly int WaterDieModifier;

		public readonly bool CanSpot;
		public readonly byte SpotRange;

		public readonly UnitConfiguration DismountAs;
		public readonly bool CanRemount;

		public readonly bool CanSupportArmored;

		public IEnumerable<UnitConfiguration> RepresentedConfigurations
		{
			get
			{
				yield return this;
				if (DismountAs != null) yield return DismountAs;
			}
		}

		public UnitConfiguration(SerializationInputStream Stream)
		{
			UniqueKey = Stream.ReadString();
			Name = Stream.ReadString();
			UnitClass = (UnitClass)Stream.ReadByte();

			PrimaryWeapon = new Weapon(Stream);
			SecondaryWeapon = new Weapon(Stream);
			Defense = Stream.ReadByte();
			Movement = Stream.ReadByte();

			CanDirectFire = Stream.ReadBoolean();
			CanIndirectFire = Stream.ReadBoolean();
			CanOverrun = Stream.ReadBoolean();
			CanCloseAssault = Stream.ReadBoolean();
			CanAirAttack = Stream.ReadBoolean();
			CanAntiAircraft = Stream.ReadBoolean();
			CanClearMines = Stream.ReadBoolean();
			CanPlaceMines = Stream.ReadBoolean();
			CanPlaceBridges = Stream.ReadBoolean();
			InnatelyClearsMines = Stream.ReadBoolean();
			ImmuneToMines = Stream.ReadBoolean();

			IsVehicle = Stream.ReadBoolean();
			IsArmored = Stream.ReadBoolean();
			LeavesWreckWhenDestroyed = Stream.ReadBoolean();
			IsEngineer = Stream.ReadBoolean();
			IsParatroop = Stream.ReadBoolean();
			IsCommando = Stream.ReadBoolean();
			HasLowProfile = Stream.ReadBoolean();

			MovementRules = Stream.ReadObject(i => new UnitMovementRules(i), false, true);

			IsCarrier = Stream.ReadBoolean();
			CanOnlyCarryInfantry = Stream.ReadBoolean();
			CanOnlyCarryLight = Stream.ReadBoolean();
			CanCarryInWater = Stream.ReadBoolean();
			CanOnlyOverrunUnarmored = Stream.ReadBoolean();
			CanOnlySupportCloseAssault = Stream.ReadBoolean();
			IsPassenger = Stream.ReadBoolean();
			IsLightPassenger = Stream.ReadBoolean();
			IsOversizedPassenger = Stream.ReadBoolean();
			CannotUseRoadMovementWithOversizedPassenger = Stream.ReadBoolean();
			OversizedPassengerMovementMultiplier = Stream.ReadFloat();
			WaterDieModifier = Stream.ReadInt32();

			CanSpot = Stream.ReadBoolean();
			SpotRange = Stream.ReadByte();

			DismountAs = Stream.ReadObject(i => new UnitConfiguration(i), true, true);
			CanRemount = Stream.ReadBoolean();

			CanSupportArmored = Stream.ReadBoolean();
		}

		public UnitConfiguration(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UniqueKey = Block.Name;
			Name = (string)attributes[(int)Attribute.NAME];
			UnitClass = (UnitClass)attributes[(int)Attribute.UNIT_CLASS];

			var weaponClass = Parse.DefaultIfNull(attributes[(int)Attribute.WEAPON_CLASS], WeaponClass.NA);
			var attack = Parse.DefaultIfNull(attributes[(int)Attribute.ATTACK], (byte)0);
			var range = Parse.DefaultIfNull(attributes[(int)Attribute.RANGE], (byte)0);
			var canDoubleRange = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_DOUBLE_RANGE], false);

			PrimaryWeapon = Parse.DefaultIfNull(
				attributes[(int)Attribute.PRIMARY_WEAPON], new Weapon(weaponClass, attack, range, canDoubleRange));
			SecondaryWeapon = Parse.DefaultIfNull(attributes[(int)Attribute.SECONDARY_WEAPON], default(Weapon));
			Defense = (byte)attributes[(int)Attribute.DEFENSE];
			Movement = Parse.DefaultIfNull(attributes[(int)Attribute.MOVEMENT], IsAircraft() ? byte.MaxValue : (byte)0);
			IsVehicle = Parse.DefaultIfNull(
				attributes[(int)Attribute.IS_VEHICLE],
				IsAircraft()
				|| UnitClass == UnitClass.AMPHIBIOUS_VEHICLE
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
			HasLowProfile = Parse.DefaultIfNull(
				attributes[(int)Attribute.HAS_LOW_PROFILE],
				UnitClass == UnitClass.INFANTRY || UnitClass == UnitClass.COMMAND_POST || UnitClass == UnitClass.FORT);

			MovementRules = Parse.DefaultIfNull(
				attributes[(int)Attribute.MOVEMENT_RULES],
				Block.Get<UnitMovementRules>(GetDefaultMovementRules()));

			IsCarrier = Parse.DefaultIfNull(
				attributes[(int)Attribute.IS_CARRIER], IsVehicle || UnitClass == UnitClass.TRANSPORT);
			CanOnlyCarryInfantry = Parse.DefaultIfNull(
				attributes[(int)Attribute.CAN_ONLY_CARRY_INFANTRY], IsCarrier && UnitClass != UnitClass.TRANSPORT);
			CanOnlyCarryLight = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_ONLY_CARRY_LIGHT], false);
			CanCarryInWater = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_CARRY_IN_WATER], false);
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
			WaterDieModifier = Parse.DefaultIfNull(attributes[(int)Attribute.WATER_DIE_MODIFIER], 0);

			IsEngineer = Parse.DefaultIfNull(attributes[(int)Attribute.IS_ENGINEER], false);
			CanDirectFire = Parse.DefaultIfNull(
				attributes[(int)Attribute.CAN_DIRECT_FIRE],
				PrimaryWeapon.Attack > 0 && UnitClass != UnitClass.MINEFIELD);
			CanIndirectFire = Parse.DefaultIfNull(
				attributes[(int)Attribute.CAN_INDIRECT_FIRE], UnitClass == UnitClass.SELF_PROPELLED_ARTILLERY);
			CanOverrun =
				Parse.DefaultIfNull(
					attributes[(int)Attribute.CAN_OVERRUN],
					IsVehicle && IsArmored && UnitClass != UnitClass.SELF_PROPELLED_ARTILLERY && CanDirectFire);
			CanOnlyOverrunUnarmored = Parse.DefaultIfNull(
				attributes[(int)Attribute.CAN_ONLY_OVERRUN_UNARMORED],
				CanOverrun && PrimaryWeapon.WeaponClass == WeaponClass.INFANTRY);
			CanCloseAssault = Parse.DefaultIfNull(
				attributes[(int)Attribute.CAN_CLOSE_ASSAULT],
				UnitClass == UnitClass.INFANTRY || UnitClass == UnitClass.CAVALRY);
			CanOnlySupportCloseAssault = Parse.DefaultIfNull(
				attributes[(int)Attribute.CAN_ONLY_SUPPORT_CLOSE_ASSAULT], false);
			CanAirAttack =
				Parse.DefaultIfNull(attributes[(int)Attribute.CAN_AIR_ATTACK], UnitClass == UnitClass.FIGHTER_BOMBER);
			CanAntiAircraft = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_ANTI_AIRCRAFT], false);
			CanClearMines = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_CLEAR_MINES], IsEngineer);
			CanPlaceMines = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_PLACE_MINES], IsEngineer);
			CanPlaceBridges = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_PLACE_BRIDGES], IsEngineer);
			InnatelyClearsMines = Parse.DefaultIfNull(attributes[(int)Attribute.INNATELY_CLEARS_MINES], false);
			ImmuneToMines =
				Parse.DefaultIfNull(attributes[(int)Attribute.IMMUNE_TO_MINES], InnatelyClearsMines || IsAircraft());

			CanSpot = Parse.DefaultIfNull(
				attributes[(int)Attribute.CAN_SPOT],
				!IsStackUnique() && UnitClass != UnitClass.FIGHTER_BOMBER && PrimaryWeapon != default(Weapon));
			SpotRange = (byte)Parse.DefaultIfNull(
				attributes[(int)Attribute.SPOT_RANGE],
				CanSpot
					? (UnitClass == UnitClass.OBSERVATION_AIRCRAFT
					   ? 30
					   : Math.Max(GetAdjustedRange(true), GetAdjustedRange(false)))
					: 0);

			DismountAs = (UnitConfiguration)attributes[(int)Attribute.DISMOUNT_AS];
			CanRemount = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_REMOUNT], DismountAs != null);

			CanSupportArmored = Parse.DefaultIfNull(attributes[(int)Attribute.CAN_SUPPORT_ARMORED], false);
		}

		string GetDefaultMovementRules()
		{
			if (IsAircraft()) return "unit-movement-rules.default-aircraft";
			if (IsVehicle) return "unit-movement-rules.default-vehicle";
			return "unit-movement-rules.default-non-vehicle";
		}

		public Weapon GetWeapon(bool Secondary)
		{
			return Secondary ? SecondaryWeapon : PrimaryWeapon;
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
			if (IsNeutral() || IsAircraft()) return 0;
			return 1;
		}

		public bool IsStackUnique()
		{
			return UnitClass == UnitClass.FORT
										 || UnitClass == UnitClass.BLOCK
										 || UnitClass == UnitClass.MINEFIELD
										 || UnitClass == UnitClass.BRIDGE;
		}

		public bool MustBeAttackedAlone()
		{
			return UnitClass == UnitClass.BRIDGE;
		}

		public BlockType GetBlockType()
		{
			if (UnitClass == UnitClass.FORT) return BlockType.NONE;
			if (UnitClass == UnitClass.MINEFIELD) return BlockType.SOFT_BLOCK;
			if (UnitClass == UnitClass.BLOCK) return BlockType.HARD_BLOCK;
			return BlockType.STANDARD;
		}

		public bool OverridesBlockType()
		{
			return UnitClass == UnitClass.FORT
										 || UnitClass == UnitClass.BLOCK
										 || UnitClass == UnitClass.MINEFIELD;
		}

		public bool IsNeutral()
		{
			return UnitClass == UnitClass.MINEFIELD
										 || UnitClass == UnitClass.BLOCK
										 || UnitClass == UnitClass.WRECKAGE
										 || UnitClass == UnitClass.BRIDGE;
		}

		public bool IsAircraft()
		{
			return UnitClass == UnitClass.OBSERVATION_AIRCRAFT || UnitClass == UnitClass.FIGHTER_BOMBER;
		}

		public bool HasUnlimitedMovement()
		{
			return Movement == byte.MaxValue;
		}

		public bool Emplaceable()
		{
			return IsNeutral() || UnitClass == UnitClass.FORT;
		}

		public byte GetAdjustedRange(bool UseSecondaryWeapon)
		{
			var weapon = GetWeapon(UseSecondaryWeapon);
			return (byte)(weapon.CanDoubleRange ? 2 * weapon.Range : weapon.Range);
		}

		public OrderInvalidReason CanDirectFireAt(bool EnemyArmored, LineOfSight LineOfSight, bool UseSecondaryWeapon)
		{
			if (!CanDirectFire) return OrderInvalidReason.UNIT_NO_ATTACK;
			if (LineOfSight.Range > GetAdjustedRange(UseSecondaryWeapon)) return OrderInvalidReason.TARGET_OUT_OF_RANGE;
			if (GetWeapon(UseSecondaryWeapon).WeaponClass == WeaponClass.INFANTRY && EnemyArmored)
				return OrderInvalidReason.TARGET_ARMORED;
			return OrderInvalidReason.NONE;
		}

		public OrderInvalidReason CanIndirectFireAt(LineOfSight LineOfSight, bool UseSecondaryWeapon)
		{
			if (LineOfSight.Range > GetAdjustedRange(UseSecondaryWeapon)) return OrderInvalidReason.TARGET_OUT_OF_RANGE;
			if (!CanIndirectFire) return OrderInvalidReason.UNIT_NO_ATTACK;
			return OrderInvalidReason.NONE;
		}

		public OrderInvalidReason CanOverrunAt(bool EnemyArmored)
		{
			if (CanOnlyOverrunUnarmored && EnemyArmored) return OrderInvalidReason.TARGET_ARMORED;
			return OrderInvalidReason.NONE;
		}

		public OrderInvalidReason CanAirAttackAt(bool EnemyArmored, LineOfSight LineOfSight, bool UseSecondaryWeapon)
		{
			if (!CanAirAttack) return OrderInvalidReason.UNIT_NO_ATTACK;
			if (LineOfSight.Range > GetAdjustedRange(UseSecondaryWeapon)) return OrderInvalidReason.TARGET_OUT_OF_RANGE;
			if (GetWeapon(UseSecondaryWeapon).WeaponClass == WeaponClass.INFANTRY && EnemyArmored)
				return OrderInvalidReason.TARGET_ARMORED;
			return OrderInvalidReason.NONE;
		}

		public OrderInvalidReason CanAntiAircraftAt(bool EnemyArmored, LineOfSight LineOfSight, bool UseSecondaryWeapon)
		{
			if (!CanAntiAircraft) return OrderInvalidReason.UNIT_NO_ATTACK;
			if (LineOfSight.Range > GetAdjustedRange(UseSecondaryWeapon)) return OrderInvalidReason.TARGET_OUT_OF_RANGE;
			if (GetWeapon(UseSecondaryWeapon).WeaponClass == WeaponClass.INFANTRY && EnemyArmored)
				return OrderInvalidReason.TARGET_ARMORED;
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
				case AttackMethod.MINEFIELD:
					return UnitClass == UnitClass.MINEFIELD
						? OrderInvalidReason.NONE
							: OrderInvalidReason.UNIT_NO_ATTACK;
				case AttackMethod.AIR:
					return CanAirAttack ? OrderInvalidReason.NONE : OrderInvalidReason.UNIT_NO_ATTACK;
				case AttackMethod.ANTI_AIRCRAFT:
					return CanAntiAircraft ? OrderInvalidReason.NONE : OrderInvalidReason.UNIT_NO_ATTACK;

				default: return OrderInvalidReason.NONE;
			}
		}

		public int GetRange(AttackMethod AttackMethod, bool UseSecondaryWeapon)
		{
			switch (AttackMethod)
			{
				case AttackMethod.OVERRUN: return 0;
				case AttackMethod.NORMAL_FIRE: return GetAdjustedRange(UseSecondaryWeapon);
				case AttackMethod.CLOSE_ASSAULT: return CanCloseAssault ? 1 : 0;
				case AttackMethod.MINEFIELD: return 0;
				case AttackMethod.AIR: return CanAirAttack ? 1 : 0;
				case AttackMethod.ANTI_AIRCRAFT: return CanAntiAircraft ? GetAdjustedRange(UseSecondaryWeapon) : 0;
			}
			// Should not end up here.
			return 0;
		}

		public float GetMaxMovement(Environment Environment)
		{
			if (HasUnlimitedMovement()) return float.MaxValue;
			if (MovementRules.IgnoresEnvironmentMovement) return Movement;
			if (Movement == 0) return 0;
			return Math.Max(1, Environment.MovementMultiplier * Movement);
		}

		public float GetPointValue(bool HalfPriceTrucks)
		{
			if (DismountAs == null) return GetPointValueInternal(HalfPriceTrucks);
			return Math.Max(GetPointValueInternal(HalfPriceTrucks), DismountAs.GetPointValue(HalfPriceTrucks));
		}

		float GetPointValueInternal(bool HalfPriceTrucks)
		{
			if (SecondaryWeapon == default(Weapon)) return GetPointValueInternal(HalfPriceTrucks, PrimaryWeapon);
			return GetPointValueInternal(HalfPriceTrucks, PrimaryWeapon)
				+ GetPointValueInternal(HalfPriceTrucks, SecondaryWeapon);
		}

		float GetPointValueInternal(bool HalfPriceTrucks, Weapon Weapon)
		{
			switch (UnitClass)
			{
				case UnitClass.TANK:
					if (Weapon.WeaponClass == WeaponClass.INFANTRY) return Defense + Movement;
					return Weapon.Attack + Weapon.Range + Defense + Movement;
				case UnitClass.ASSAULT_GUN:
					if (CanAntiAircraft)
					{
						if (Weapon.WeaponClass == WeaponClass.INFANTRY)
							return .5f * Weapon.Attack + .5f * Weapon.Range + Defense + Movement;
						return 1.5f * Weapon.Attack + .5f * Weapon.Range + Defense + Movement;
					}
					return Weapon.Attack + Math.Min((int)Defense, 6) + Defense + Movement;
				case UnitClass.FLAME_TANK:
					return .5f * Weapon.Attack + Weapon.Range + Defense + Movement;
				case UnitClass.SELF_PROPELLED_ARTILLERY:
					if (Weapon.Range > 16)
						return Weapon.Attack + .25f * Weapon.Range + Defense + Movement;
					return Weapon.Attack + 4 + Defense + Movement;
				case UnitClass.RECONNAISSANCE_VEHICLE:
					if (CanSpot) return 5 + Defense + Movement;
					if (CanAntiAircraft)
					{
						if (Weapon.WeaponClass == WeaponClass.INFANTRY)
							return .5f * Weapon.Attack + .5f * Weapon.Range + Defense + Movement;
						return 1.5f * Weapon.Attack + .5f * Weapon.Range + Defense + Movement;
					}
					if (Weapon.WeaponClass == WeaponClass.INFANTRY) return Defense + Movement;
					if (Weapon.WeaponClass == WeaponClass.ANTI_ARMOR)
						return Weapon.Attack + Weapon.Range + Defense + Movement;
					return Weapon.Attack + Math.Min((int)Weapon.Range, 6) + Defense + Movement;
				case UnitClass.TRANSPORT:
					if (IsVehicle)
					{
						if (IsArmored) return Weapon.Attack + Weapon.Range + Defense + Movement;
						return (HalfPriceTrucks ? .5f : 1)
							* (Weapon.Attack + Weapon.Range + Defense + .5f * Movement);
					}
					return Weapon.Attack + Weapon.Range + Defense + Movement;
				case UnitClass.TOWED_GUN:
					if (CanIndirectFire && Weapon.WeaponClass == WeaponClass.HIGH_EXPLOSIVE)
						return .5f * Weapon.Attack + .25f * Weapon.Range + Defense + Movement;
					if (CanAntiAircraft)
					{
						if (Weapon.WeaponClass == WeaponClass.INFANTRY)
							return .5f * Weapon.Attack + .5f + Weapon.Range + Defense + Movement;
						return Weapon.Attack + .5f * Weapon.Range + Defense + Movement;
					}
					if (Weapon.WeaponClass == WeaponClass.HIGH_EXPLOSIVE)
						return .5f * Weapon.Attack + Math.Min((int)Weapon.Range, 6) + Defense + Movement;
					return .5f * Weapon.Attack + .5f * Weapon.Range + Defense + Movement;
				case UnitClass.INFANTRY:
					if (IsEngineer) return 2 * Weapon.Attack + 1 + Defense + Movement;
					if (IsCommando) return 2 * Weapon.Attack + 1 + Defense + 2 * Movement;
					return Weapon.Attack + 1 + Defense + Movement;
				case UnitClass.CAVALRY:
					return Weapon.Attack + 1 + Defense + 1;
				case UnitClass.COMMAND_POST:
					return 1;
				case UnitClass.AMPHIBIOUS_VEHICLE:
					if (!CanOnlyCarryInfantry || Weapon.WeaponClass == WeaponClass.INFANTRY)
						return Defense + 1.5f * Movement;
					if (Weapon.WeaponClass == WeaponClass.HIGH_EXPLOSIVE)
						return Weapon.Attack + Math.Min((int)Weapon.Range, 6) + Defense + 1.5f * Movement;
					return Weapon.Attack + Weapon.Range + Defense + 1.5f * Movement;
				case UnitClass.ENGINEER_VEHICLE:
					return Weapon.Attack + Weapon.Range + Defense + 1.5f * Movement;
				case UnitClass.BRIDGE:
					return Defense;
				case UnitClass.FORT:
					return 10 + .5f * Defense;
				case UnitClass.BLOCK:
					return 12;
				case UnitClass.MINEFIELD:
					return 10 * Weapon.Attack + 15;
				case UnitClass.OBSERVATION_AIRCRAFT:
					return 50;
				case UnitClass.FIGHTER_BOMBER:
					if (Weapon.WeaponClass == WeaponClass.INFANTRY) return 0;
					if (Weapon.WeaponClass == WeaponClass.ANTI_ARMOR) return 3 * Weapon.Attack;
					return Weapon.Attack;
				default:
					return Weapon.Attack + Weapon.Range + Defense + Movement;
			}
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueKey);
			Stream.Write(Name);
			Stream.Write((byte)UnitClass);

			Stream.Write(PrimaryWeapon);
			Stream.Write(SecondaryWeapon);
			Stream.Write(Defense);
			Stream.Write(Movement);

			Stream.Write(CanDirectFire);
			Stream.Write(CanIndirectFire);
			Stream.Write(CanOverrun);
			Stream.Write(CanCloseAssault);
			Stream.Write(CanAirAttack);
			Stream.Write(CanAntiAircraft);
			Stream.Write(CanClearMines);
			Stream.Write(CanPlaceMines);
			Stream.Write(CanPlaceBridges);
			Stream.Write(InnatelyClearsMines);
			Stream.Write(ImmuneToMines);

			Stream.Write(IsVehicle);
			Stream.Write(IsArmored);
			Stream.Write(LeavesWreckWhenDestroyed);
			Stream.Write(IsEngineer);
			Stream.Write(IsParatroop);
			Stream.Write(IsCommando);
			Stream.Write(HasLowProfile);

			Stream.Write(MovementRules, false, true);

			Stream.Write(IsCarrier);
			Stream.Write(CanOnlyCarryInfantry);
			Stream.Write(CanOnlyCarryLight);
			Stream.Write(CanCarryInWater);
			Stream.Write(CanOnlyOverrunUnarmored);
			Stream.Write(CanOnlySupportCloseAssault);
			Stream.Write(IsPassenger);
			Stream.Write(IsLightPassenger);
			Stream.Write(IsOversizedPassenger);
			Stream.Write(CannotUseRoadMovementWithOversizedPassenger);
			Stream.Write(OversizedPassengerMovementMultiplier);
			Stream.Write(WaterDieModifier);

			Stream.Write(CanSpot);
			Stream.Write(SpotRange);

			Stream.Write(DismountAs, true, true);
			Stream.Write(CanRemount);

			Stream.Write(CanSupportArmored);
		}

		public override string ToString()
		{
			return string.Format("[UnitConfiguration: Name={0}]", Name);
		}
	}
}
