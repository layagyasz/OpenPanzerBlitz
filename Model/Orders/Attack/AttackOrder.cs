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
		IEnumerable<SingleAttackOrder> Attackers { get; }
		IEnumerable<OddsCalculation> OddsCalculations { get; }
		CombatResultsTable CombatResultsTable { get; }

		bool IsCompatible(SingleAttackOrder AttackOrder);
		OrderInvalidReason AddAttacker(SingleAttackOrder AttackOrder);
		void RemoveAttacker(Unit Attacker);
		void SetAttackTarget(AttackTarget Target);
	}
}
