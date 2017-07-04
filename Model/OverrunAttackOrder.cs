using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class OverrunAttackOrder : AttackOrder
	{
		List<OverrunMoveOrder> _Moves = new List<OverrunMoveOrder>();

		public OverrunAttackOrder(Army AttackingArmy, Tile AttackAt)
			: base(AttackingArmy, AttackAt, AttackMethod.OVERRUN)
		{
		}

		public override NoSingleAttackReason AddAttacker(Unit Attacker)
		{
			throw new Exception("Use AddAttacker(Unit, OverrunMoveOrder) for Overrun attacks");
		}

		public NoSingleAttackReason AddAttacker(Unit Attacker, OverrunMoveOrder MoveOrder)
		{
			NoSingleAttackReason r = base.AddAttacker(Attacker);
			if (r != NoSingleAttackReason.NONE) _Moves.Add(MoveOrder);
			return r;
		}

		public override NoAttackReason Validate()
		{
			return base.Validate();
		}

		public override bool Execute(Random Random)
		{
			return base.Execute(Random);
		}
	}
}
