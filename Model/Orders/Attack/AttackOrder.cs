using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public interface AttackOrder : Order
	{
		EventHandler<EventArgs> OnChanged { get; set; }

		Tile TargetTile { get; }
		AttackMethod AttackMethod { get; }
		AttackTarget Target { get; }
		IEnumerable<OddsCalculation> OddsCalculations { get; }

		OrderInvalidReason AddAttacker(SingleAttackOrder AttackOrder);
		void RemoveAttacker(Unit Attacker);
		void SetAttackTarget(AttackTarget Target);
	}
}
