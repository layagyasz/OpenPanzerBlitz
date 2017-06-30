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
		List<Unit> _Attackers = new List<Unit>();

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

		public NoSingleAttackReason AddAttacker(Unit Attacker)
		{
			if (!_Attackers.Contains(Attacker))
			{
				NoSingleAttackReason canAttack = Attacker.UnitConfiguration.CanAttack(
					AttackMethod, AttackAt.Units.Count(
						i => i.UnitConfiguration.IsArmored) > AttackAt.Units.Count(i => !i.UnitConfiguration.IsArmored),
					Attacker.GetLineOfSight(AttackAt));
				if (canAttack != NoSingleAttackReason.NONE) return canAttack;

				_Attackers.Add(Attacker);
				Recalculate();
				return NoSingleAttackReason.NONE;
			}
			return NoSingleAttackReason.DUPLICATE;
		}

		public void RemoveAttacker(Unit Attacker)
		{
			_Attackers.Remove(Attacker);
			Recalculate();
		}

		private void Recalculate()
		{
			if (_Attackers.Count == 0)
			{
				_OddsCalculations.Clear();
				return;
			}

			if (AttackTarget == AttackTarget.ALL)
			{
				_OddsCalculations.Clear();
				_OddsCalculations.Add(
					new OddsCalculation(
						_Attackers.Select(
							i => new Tuple<Unit, LineOfSight>(i, i.GetLineOfSight(AttackAt))),
						AttackAt.Units.ToList(),
						AttackMethod,
						AttackAt));
			}
			else if (AttackTarget == AttackTarget.WEAKEST)
			{
				_OddsCalculations.Clear();
				_OddsCalculations.Add(AttackAt.Units.Select(i =>
					new OddsCalculation(
						_Attackers.Select(
					   		j => new Tuple<Unit, LineOfSight>(j, j.GetLineOfSight(AttackAt))),
										   new Unit[] { i },
											AttackMethod,
											AttackAt))
									 .ArgMax(i => OddsIndex(i.Odds, i.OddsAgainst)));
			}
		}

		public NoAttackReason Validate()
		{
			return NoAttackReason.NONE;
		}

		public bool Execute(Random Random)
		{
			if (Validate() != NoAttackReason.NONE) return false;

			_Attackers.ForEach(i => i.Fire());
			foreach (OddsCalculation c in _OddsCalculations)
			{
				CombatResult result = COMBAT_RESULTS[
					OddsIndex(c.Odds, c.OddsAgainst),
					Random.Next(2, 7) + c.DieModifier];
				foreach (Unit u in c.Defenders) u.HandleCombatResult(result);
			}
			AttackAt.FireAt();
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
