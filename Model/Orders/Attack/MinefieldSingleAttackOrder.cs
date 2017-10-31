﻿using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class MinefieldSingleAttackOrder : SingleAttackOrder
	{
		Unit _Attacker;
		Unit _Defender;

		public override Unit Attacker
		{
			get
			{
				return _Attacker;
			}
		}

		public override Unit Defender
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

		public MinefieldSingleAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Unit)Objects[Stream.ReadInt32()], (Unit)Objects[Stream.ReadInt32()]) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(_Attacker.Id);
			Stream.Write(_Defender.Id);
		}

		public override void SetTreatStackAsArmored(bool TreatStackAsArmored)
		{
			// Armored doesn't matter.
		}

		public override AttackFactorCalculation GetAttack()
		{
			return new AttackFactorCalculation(
				_Defender.Configuration.Defense * _Attacker.Configuration.Attack,
				new List<AttackFactorCalculationFactor>() { AttackFactorCalculationFactor.MINEFIELD });
		}

		public override OrderInvalidReason Validate()
		{
			if (_Attacker.CanAttack(AttackMethod.MINEFIELD) != OrderInvalidReason.NONE)
				return OrderInvalidReason.UNIT_NO_ATTACK;
			if (_Attacker.Position != _Defender.Position) return OrderInvalidReason.TARGET_OUT_OF_RANGE;
			return OrderInvalidReason.NONE;
		}

		public override OrderStatus Execute(Random Random)
		{
			return Validate() == OrderInvalidReason.NONE ? OrderStatus.FINISHED : OrderStatus.ILLEGAL;
		}
	}
}
