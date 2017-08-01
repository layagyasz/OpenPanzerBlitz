using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class AttackOrder : Order
	{
		private static readonly CombatResult[,] COMBAT_RESULTS =
		{
			{
				CombatResult.DISRUPT,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS
			},
			{
				CombatResult.DOUBLE_DISRUPT,
				CombatResult.DISRUPT,
				CombatResult.DISRUPT,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS
			},
			{
				CombatResult.DESTROY,
				CombatResult.DOUBLE_DISRUPT,
				CombatResult.DISRUPT,
				CombatResult.DISRUPT,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS
			},
			{
				CombatResult.DESTROY,
				CombatResult.DOUBLE_DISRUPT,
				CombatResult.DOUBLE_DISRUPT,
				CombatResult.DISRUPT,
				CombatResult.DISRUPT,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS

			},
			{
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DOUBLE_DISRUPT,
				CombatResult.DOUBLE_DISRUPT,
				CombatResult.DISRUPT,
				CombatResult.DISRUPT,
				CombatResult.MISS,
				CombatResult.MISS
			},
			{
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DOUBLE_DISRUPT,
				CombatResult.DOUBLE_DISRUPT,
				CombatResult.MISS,
				CombatResult.MISS
			},
			{
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.MISS
			},
		};

		public readonly Army AttackingArmy;
		public readonly Tile AttackAt;
		public readonly AttackMethod AttackMethod;

		AttackTarget _AttackTarget = AttackTarget.ALL;
		List<SingleAttackOrder> _Attackers = new List<SingleAttackOrder>();
		List<OddsCalculation> _OddsCalculations = new List<OddsCalculation>();

		public AttackTarget AttackTarget
		{
			get
			{
				return _AttackTarget;
			}
		}
		public IEnumerable<OddsCalculation> OddsCalculations
		{
			get
			{
				return _OddsCalculations;
			}
		}

		public AttackOrder(Army AttackingArmy, Tile AttackAt, AttackMethod AttackMethod)
		{
			this.AttackingArmy = AttackingArmy;
			this.AttackAt = AttackAt;
			this.AttackMethod = AttackMethod;
		}

		public void SetAttackTarget(AttackTarget AttackTarget)
		{
			_AttackTarget = AttackTarget;
			Recalculate();
		}

		public NoSingleAttackReason AddAttacker(SingleAttackOrder AttackOrder)
		{
			if (!_Attackers.Any(i => i.Attacker == AttackOrder.Attacker))
			{
				NoSingleAttackReason canAttack = AttackOrder.Validate();
				if (canAttack != NoSingleAttackReason.NONE) return canAttack;

				_Attackers.Add(AttackOrder);
				Recalculate();
				return NoSingleAttackReason.NONE;
			}
			return NoSingleAttackReason.DUPLICATE;
		}

		public void RemoveAttacker(Unit Attacker)
		{
			_Attackers.RemoveAll(i => i.Attacker == Attacker);
			Recalculate();
		}

		void Recalculate()
		{
			_OddsCalculations.Clear();
			if (_Attackers.Count == 0) return;

			if (AttackTarget == AttackTarget.ALL)
			{

				_OddsCalculations.Add(
					new OddsCalculation(
						_Attackers,
						AttackAt.Units.Where(i => i.CanBeAttackedBy(AttackingArmy) == NoSingleAttackReason.NONE),
						AttackMethod,
						AttackAt));
			}
			else if (AttackTarget == AttackTarget.WEAKEST)
			{
				_OddsCalculations.Add(
					AttackAt.Units
						.Where(i => i.CanBeAttackedBy(AttackingArmy) == NoSingleAttackReason.NONE)
						.Select(
							i => new OddsCalculation(
								_Attackers,
								new Unit[] { i },
								AttackMethod,
								AttackAt))
						.ArgMax(i => OddsIndex(i.Odds, i.OddsAgainst)));
			}
			else
			{
				_OddsCalculations.AddRange(
					_Attackers
						.GroupBy(i => i.Defender)
						.Select(i => new OddsCalculation(i, new Unit[] { i.Key }, AttackMethod, AttackAt)));
				_OddsCalculations.Sort(
					(x, y) => OddsIndex(x.Odds, x.OddsAgainst).CompareTo(OddsIndex(y.Odds, y.OddsAgainst)));
			}
		}

		public virtual NoAttackReason Validate()
		{
			if (MustAttackAllUnits() && AttackTarget != AttackTarget.ALL) return NoAttackReason.MUST_ATTACK_ALL;
			if (_Attackers.Any(i => i.Validate() != NoSingleAttackReason.NONE)) return NoAttackReason.ILLEGAL;
			if (AttackAt.CanBeAttacked(AttackMethod) != NoAttackReason.NONE) return AttackAt.CanBeAttacked(AttackMethod);
			if (_AttackTarget == AttackTarget.EACH)
			{
				if (_OddsCalculations.Count != AttackAt.Units.Count(
					i => i.CanBeAttackedBy(AttackingArmy) == NoSingleAttackReason.NONE))
					return NoAttackReason.ILLEGAL_EACH;
				if (_OddsCalculations.Any(i => OddsIndex(i.Odds, i.OddsAgainst) < 3))
					return NoAttackReason.ILLEGAL_EACH;
			}

			return NoAttackReason.NONE;
		}

		private bool MustAttackAllUnits()
		{
			return AttackMethod != AttackMethod.NORMAL_FIRE
											   || AttackAt.Configuration.MustAttackAllUnits
											   || AttackAt.Units.Any(i => i.Configuration.UnitClass == UnitClass.FORT);
		}

		public virtual bool Execute(Random Random)
		{
			if (Validate() != NoAttackReason.NONE) return false;

			_Attackers.ForEach(i => i.Execute(Random));
			foreach (OddsCalculation c in _OddsCalculations)
			{
				CombatResult result = COMBAT_RESULTS[
					OddsIndex(c.Odds, c.OddsAgainst),
					Random.Next(2, 7) + c.DieModifier];
				foreach (Unit u in c.Defenders) u.HandleCombatResult(result);
			}
			if (AttackMethod != AttackMethod.MINEFIELD) AttackAt.FireAt();
			return true;
		}

		// Map 4, 3, 2, 1, 2, 3, 4 => 0 -> 6
		private int OddsIndex(int Odds, bool OddsAgainst)
		{
			if (OddsAgainst) return 4 - Odds;
			else return 2 + Odds;
		}
	}
}
