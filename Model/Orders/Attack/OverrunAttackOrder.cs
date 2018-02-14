using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class OverrunAttackOrder : AttackOrderBase<OverrunSingleAttackOrder>
	{
		public override AttackMethod AttackMethod
		{
			get
			{
				return AttackMethod.OVERRUN;
			}
		}

		public OverrunAttackOrder(Army Army, Tile TargetTile)
			: base(Army, TargetTile) { }

		public OverrunAttackOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: base(Stream, Objects)
		{
			_Attackers = Stream.ReadEnumerable(i => new OverrunSingleAttackOrder(Stream, Objects)).ToList();
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			Stream.Write(_Attackers);
		}

		public override bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.VEHICLE_COMBAT_MOVEMENT;
		}

		public override OrderInvalidReason AddAttacker(OverrunSingleAttackOrder AttackOrder)
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

			if (!_Attackers.All(i => i is OverrunSingleAttackOrder)) return OrderInvalidReason.ILLEGAL;
			foreach (var g in _Attackers.Cast<OverrunSingleAttackOrder>().GroupBy(i => i.ExitTile))
			{
				if (g.Key.GetStackSize() +
					g.Sum(i => i.ExitTile.Units.Contains(i.Attacker) ? 0 : i.Attacker.GetStackSize())
					> Army.Configuration.Faction.StackLimit)
					return OrderInvalidReason.OVERRUN_EXIT;
			}

			return base.Validate();
		}
	}
}
