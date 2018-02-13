﻿using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class MinefieldSingleAttackOrder : SingleAttackOrder
	{
		public MinefieldSingleAttackOrder(Unit Attacker, Unit Defender)
			: base(Attacker, Defender) { }

		public MinefieldSingleAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Unit)Objects[Stream.ReadInt32()], (Unit)Objects[Stream.ReadInt32()]) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Attacker.Id);
			Stream.Write(Defender.Id);
		}

		public override AttackFactorCalculation GetAttack()
		{
			return new AttackFactorCalculation(
				Defender.Configuration.Defense * Attacker.Configuration.Attack,
				new List<AttackFactorCalculationFactor>() { AttackFactorCalculationFactor.MINEFIELD });
		}

		public override AttackOrder GenerateNewAttackOrder()
		{
			return new MinefieldAttackOrder(Army, Attacker.Position);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return true;
		}

		public override OrderInvalidReason Validate()
		{
			if (Attacker.CanAttack(AttackMethod.MINEFIELD) != OrderInvalidReason.NONE)
				return OrderInvalidReason.UNIT_NO_ATTACK;
			if (Attacker.Position != Defender.Position) return OrderInvalidReason.TARGET_OUT_OF_RANGE;
			if (Attacker.HasInteraction<ClearMinefieldInteraction>(i => i.Agent == Defender) != null)
				return OrderInvalidReason.TARGET_IMMUNE;
			OrderInvalidReason r = Defender.CanBeAttackedBy(Army, AttackMethod.MINEFIELD);
			if (r != OrderInvalidReason.NONE) return r;
			return OrderInvalidReason.NONE;
		}

		public override OrderStatus Execute(Random Random)
		{
			return Validate() == OrderInvalidReason.NONE ? OrderStatus.FINISHED : OrderStatus.ILLEGAL;
		}

		public override string ToString()
		{
			return string.Format("[MinefieldSingleAttackOrder: Attacker={0}, Defender={1}]", Attacker, Defender);
		}
	}
}
