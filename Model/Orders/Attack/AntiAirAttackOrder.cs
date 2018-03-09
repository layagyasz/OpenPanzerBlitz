using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class AntiAirAttackOrder : AttackOrderBase<AntiAirSingleAttackOrder>
	{
		public override AttackMethod AttackMethod
		{
			get
			{
				return AttackMethod.ANTI_AIRCRAFT;
			}
		}

		public override CombatResultsTable CombatResultsTable
		{
			get
			{
				return CombatResultsTable.AA_CRT;
			}
		}

		public AntiAirAttackOrder(Army Army, Tile TargetTile)
			: base(Army, TargetTile) { }

		public AntiAirAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: base(Stream, Objects)
		{
			_Attackers = Stream.ReadEnumerable(i => new AntiAirSingleAttackOrder(Stream, Objects)).ToList();
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

		public override OrderInvalidReason AddAttacker(AntiAirSingleAttackOrder AttackOrder)
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

			return base.Validate();
		}
	}
}
