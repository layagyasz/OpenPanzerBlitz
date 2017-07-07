using System;
using System.Collections.Generic;
using System.Linq;

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
			if (r == NoSingleAttackReason.NONE) _Moves.Add(MoveOrder);
			return r;
		}

		public override NoAttackReason Validate()
		{
			return base.Validate();
		}

		public override bool Execute(Random Random)
		{
			if (Validate() != NoAttackReason.NONE) return false;

			return _Moves.All(i => i.Execute(Random)) && base.Execute(Random);
		}
	}
}
