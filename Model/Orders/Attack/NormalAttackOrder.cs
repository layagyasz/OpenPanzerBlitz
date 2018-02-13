﻿using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class NormalAttackOrder : AttackOrderBase<NormalSingleAttackOrder>
	{
		public override AttackMethod AttackMethod
		{
			get
			{
				return AttackMethod.NORMAL_FIRE;
			}
		}

		public NormalAttackOrder(Army AttackingArmy, Tile TargetTile)
			: base(AttackingArmy, TargetTile) { }

		public NormalAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: base(Stream, Objects)
		{
			_Attackers = Stream.ReadEnumerable(i => new NormalSingleAttackOrder(Stream, Objects)).ToList();
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			Stream.Write(_Attackers);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.ATTACK;
		}

		public override OrderInvalidReason AddAttacker(NormalSingleAttackOrder AttackOrder)
		{
			if (!_Attackers.Any(i => i.Attacker == AttackOrder.Attacker))
			{
				return base.AddAttacker(AttackOrder);
			}
			return OrderInvalidReason.UNIT_DUPLICATE;
		}

		public override OrderInvalidReason Validate()
		{
			if (Target != AttackTarget.ALL && (TargetTile.RulesCalculator.MustAttackAllUnits
				 || TargetTile.Units.Any(i => i.Configuration.UnitClass == UnitClass.FORT)))
				return OrderInvalidReason.MUST_ATTACK_ALL;

			if (Target == AttackTarget.EACH)
			{
				foreach (NormalSingleAttackOrder attacker in _Attackers)
				{
					OrderInvalidReason r = attacker.Defender.CanBeAttackedBy(Army, AttackMethod);
					if (r != OrderInvalidReason.NONE) return r;
				}
				if (_OddsCalculations.Count != TargetTile.Units.Count(
					i => i.CanBeAttackedBy(Army, AttackMethod) == OrderInvalidReason.NONE))
					return OrderInvalidReason.ILLEGAL_ATTACK_EACH;
				if (_OddsCalculations.Any(i => i.GetOddsIndex() < 3))
					return OrderInvalidReason.ILLEGAL_ATTACK_EACH;
			}

			return base.Validate();
		}
	}
}
