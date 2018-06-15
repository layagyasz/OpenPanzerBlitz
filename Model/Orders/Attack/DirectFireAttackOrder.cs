using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class DirectFireAttackOrder : AttackOrderBase<DirectFireSingleAttackOrder>
	{
		public override AttackMethod AttackMethod
		{
			get
			{
				return AttackMethod.DIRECT_FIRE;
			}
		}

		public override bool ResultPerDefender
		{
			get
			{
				return false;
			}
		}

		public DirectFireAttackOrder(Army Army, Tile TargetTile)
			: base(Army, TargetTile) { }

		public DirectFireAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: base(Stream, Objects)
		{
			_Attackers = Stream.ReadEnumerable(i => new DirectFireSingleAttackOrder(Stream, Objects)).ToList();
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

		public override OrderInvalidReason Validate()
		{
			if (Target != AttackTarget.ALL && TargetTile.Rules.MustAttackAllUnits)
				return OrderInvalidReason.MUST_ATTACK_ALL;

			if (Target == AttackTarget.EACH)
			{
				foreach (DirectFireSingleAttackOrder attacker in _Attackers)
				{
					var r = attacker.Defender.CanBeAttackedBy(Army, AttackMethod);
					if (r != OrderInvalidReason.NONE) return r;
				}
				if (_OddsCalculations.Count != TargetTile.Units.Count(
					i => i.CanBeAttackedBy(Army, AttackMethod) == OrderInvalidReason.NONE))
					return OrderInvalidReason.ILLEGAL_ATTACK_EACH;
				if (_OddsCalculations.Any(i => i.Odds > 1 && i.OddsAgainst))
					return OrderInvalidReason.ILLEGAL_ATTACK_EACH;
			}

			return base.Validate();
		}
	}
}
