using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class MinefieldAttackOrder : AttackOrderBase<MinefieldSingleAttackOrder>
	{
		public override AttackMethod AttackMethod
		{
			get
			{
				return AttackMethod.MINEFIELD;
			}
		}

		public MinefieldAttackOrder(Army Army, Tile TargetTile)
			: base(Army, TargetTile)
		{
			Target = AttackTarget.EACH;
		}

		public MinefieldAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: base(Stream, Objects)
		{
			_Attackers = Stream.ReadEnumerable(i => new MinefieldSingleAttackOrder(Stream, Objects)).ToList();
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			Stream.Write(_Attackers);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.MINEFIELD_ATTACK;
		}

		public override OrderInvalidReason AddAttacker(MinefieldSingleAttackOrder AttackOrder)
		{
			if (!_Attackers.Any(i => i.Attacker == AttackOrder.Attacker))
			{
				return base.AddAttacker(AttackOrder);
			}
			return OrderInvalidReason.UNIT_DUPLICATE;
		}

		public override OrderInvalidReason Validate()
		{
			if (Target != AttackTarget.EACH) return OrderInvalidReason.MUST_ATTACK_EACH;

			foreach (MinefieldSingleAttackOrder attacker in _Attackers)
			{
				if (attacker.Attacker.Position != TargetTile) return OrderInvalidReason.ILLEGAL;
			}

			return base.Validate();
		}
	}
}
