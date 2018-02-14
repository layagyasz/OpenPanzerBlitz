using System;
using System.Collections.Generic;
using System.Linq;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class CloseAssaultAttackOrder : AttackOrderBase<CloseAssaultSingleAttackOrder>
	{
		public override AttackMethod AttackMethod
		{
			get
			{
				return AttackMethod.CLOSE_ASSAULT;
			}
		}

		public CloseAssaultAttackOrder(Army Army, Tile TargetTile)
			: base(Army, TargetTile) { }

		public CloseAssaultAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: base(Stream, Objects)
		{
			_Attackers = Stream.ReadEnumerable(i => new CloseAssaultSingleAttackOrder(Stream, Objects)).ToList();
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			Stream.Write(_Attackers);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.CLOSE_ASSAULT;
		}

		public override OrderInvalidReason AddAttacker(CloseAssaultSingleAttackOrder AttackOrder)
		{
			if (!_Attackers.Any(i => i.Attacker == AttackOrder.Attacker))
			{
				return base.AddAttacker(AttackOrder);
			}
			return OrderInvalidReason.UNIT_DUPLICATE;
		}

		public override OrderInvalidReason Validate()
		{
			if (Target != AttackTarget.ALL) return OrderInvalidReason.MUST_ATTACK_ALL;

			foreach (SingleAttackOrder attacker in _Attackers)
			{
				if (attacker.Attacker.Configuration.CanOnlySupportCloseAssault
					&& !_Attackers.Any(
						i => i.Attacker.Position == attacker.Attacker.Position
						&& !i.Attacker.Configuration.CanOnlySupportCloseAssault))
					return OrderInvalidReason.UNIT_CLOSE_ASSAULT_SUPPORT;
			}

			return base.Validate();
		}
	}
}
