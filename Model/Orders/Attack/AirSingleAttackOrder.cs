﻿using System;
using System.Collections.Generic;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class AirSingleAttackOrder : SingleAttackOrder
	{
		public override Tile AttackTile
		{
			get
			{
				return Defender.Position;
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		public AirSingleAttackOrder(Unit Attacker, Unit Defender, bool UseSecondaryWeapon)
			: base(Attacker, Defender, UseSecondaryWeapon) { }

		public AirSingleAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Unit)Objects[Stream.ReadInt32()], (Unit)Objects[Stream.ReadInt32()], Stream.ReadBoolean()) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Attacker.Id);
			Stream.Write(Defender.Id);
			Stream.Write(UseSecondaryWeapon);
		}

		public override AttackFactorCalculation GetAttack()
		{
			if (Validate() == OrderInvalidReason.NONE)
				return new AttackFactorCalculation(
					Attacker, AttackMethod.AIR, TreatStackAsArmored, null, UseSecondaryWeapon);
			return new AttackFactorCalculation(
				0,
				new List<AttackFactorCalculationFactor> { AttackFactorCalculationFactor.CANNOT_ATTACK });
		}

		public override AttackOrder GenerateNewAttackOrder()
		{
			return new AirAttackOrder(Army, Defender.Position);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.AIRCRAFT;
		}

		public override OrderInvalidReason Validate()
		{
			if (Defender == null) return OrderInvalidReason.ILLEGAL;
			if (Attacker.Position.HexCoordinate.Distance(Defender.Position.HexCoordinate) > 1)
				return OrderInvalidReason.TARGET_OUT_OF_RANGE;

			return Attacker.CanAttack(AttackMethod.AIR, TreatStackAsArmored, null, UseSecondaryWeapon);
		}

		public override OrderStatus Execute(Random Random)
		{
			Attacker.Fire(AttackTile, UseSecondaryWeapon);
			return OrderStatus.FINISHED;
		}

		public override string ToString()
		{
			return string.Format("[AirSingleAttackOrder: Attacker={0}, Defender={1}]", Attacker, Defender);
		}
	}
}
