using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class AirAttackOrder : AttackOrderBase<AirSingleAttackOrder>
	{
		public override AttackMethod AttackMethod
		{
			get
			{
				return AttackMethod.AIR;
			}
		}

		public override bool ResultPerDefender
		{
			get
			{
				return false;
			}
		}

		public AirAttackOrder(Army Army, Tile TargetTile)
			: base(Army, TargetTile) { }

		public AirAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: base(Stream, Objects)
		{
			_Attackers = Stream.ReadEnumerable(i => new AirSingleAttackOrder(Stream, Objects)).ToList();
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			Stream.Write(_Attackers);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.AIRCRAFT;
		}

		public override OrderInvalidReason Validate()
		{
			if (Target != AttackTarget.ALL && (TargetTile.Rules.MustAttackAllUnits
				 || TargetTile.Units.Any(i => i.Configuration.UnitClass == UnitClass.FORT)))
				return OrderInvalidReason.MUST_ATTACK_ALL;

			if (Target == AttackTarget.EACH)
			{
				foreach (var attacker in _Attackers)
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
