using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class OddsCalculation : IComparable
	{
		public readonly List<Tuple<SingleAttackOrder, AttackFactorCalculation>> AttackFactorCalculations;
		public readonly List<Unit> Defenders;
		public readonly List<OddsCalculationFactor> OddsCalculationFactors;
		public readonly bool StackArmored;
		public readonly int TotalAttack;
		public readonly int TotalDefense;

		bool _OddsAgainst;
		int _Odds;
		int _DieModifier;

		public bool OddsAgainst
		{
			get
			{
				return _OddsAgainst;
			}
		}
		public int Odds
		{
			get
			{
				return _Odds;
			}
		}
		public int DieModifier
		{
			get
			{
				return _DieModifier;
			}
		}

		public OddsCalculation(
			IEnumerable<SingleAttackOrder> AttackOrders,
			IEnumerable<Unit> Defenders,
			AttackMethod AttackMethod,
			Tile Tile,
			int OddsClamp)
		{
			OddsCalculationFactors = new List<OddsCalculationFactor>();

			StackArmored = Defenders.Any(i => i.Configuration.UnitClass == UnitClass.FORT)
					 			|| Tile.Rules.TreatUnitsAsArmored
								|| TreatStackAsArmored(AttackOrders, Defenders);

			TotalDefense = Defenders.Sum(i => i.Configuration.Defense);
			foreach (SingleAttackOrder a in AttackOrders) a.TreatStackAsArmored = StackArmored;
			AttackFactorCalculations = AttackOrders.Select(
				i => new Tuple<SingleAttackOrder, AttackFactorCalculation>(
					i, i.GetAttack())).ToList();
			this.Defenders = Defenders.ToList();

			// Calculate Initial Odds
			TotalAttack = AttackFactorCalculations.Sum(i => i.Item2.Attack);

			if (TotalAttack == 0 || TotalDefense == 0) return;

			if (TotalAttack >= TotalDefense)
			{
				_Odds = TotalAttack / TotalDefense;
			}
			else
			{
				_Odds = (int)Math.Ceiling((float)TotalDefense / TotalAttack);
				_OddsAgainst = true;
			}

			_DieModifier = 0;
			// Modifiers for special attacks.
			if (AttackMethod == AttackMethod.CLOSE_ASSAULT)
			{
				_DieModifier -= 2;
				OddsCalculationFactors.Add(OddsCalculationFactor.CLOSE_ASSAULT);
				if (AttackOrders
					.Select(i => i.Attacker)
					.GroupBy(i => i.Position)
					.Any(i => i.Count(j => j.Configuration.IsEngineer) > 0 && i.Count() > 1))
				{
					OddsCalculationFactors.Add(OddsCalculationFactor.CLOSE_ASSAULT_ENGINEERS);
					IncreaseOdds();
				}
			}
			else if (AttackMethod == AttackMethod.OVERRUN)
			{
				_DieModifier -= 2;
				OddsCalculationFactors.Add(OddsCalculationFactor.OVERRUN);
				IncreaseOdds();
			}

			if (AttackMethod != AttackMethod.ANTI_AIRCRAFT && AttackMethod != AttackMethod.MINEFIELD)
			{
				// Terrain modifiers.
				int totalDieModifier = Defenders.First().Position.Rules.DieModifier
												+ Defenders.Min(i => i.Configuration.WaterDieModifier);
				if (totalDieModifier != 0)
				{
					_DieModifier += totalDieModifier;
					OddsCalculationFactors.Add(OddsCalculationFactor.TERRAIN);
				}
				// Disrupted modifier.
				if (Defenders.Any(i => i.Status == UnitStatus.DISRUPTED))
				{
					_DieModifier -= 1;
					OddsCalculationFactors.Add(OddsCalculationFactor.DISRUPTED);
				}
			}

			// Clamp the die modifier.
			if (_DieModifier < -2) _DieModifier = -2;
			if (_DieModifier > 1) _DieModifier = 1;

			// Clamp the odds.
			if (_Odds > OddsClamp) _Odds = OddsClamp;
		}

		void IncreaseOdds()
		{
			if (OddsAgainst) --_Odds;
			else ++_Odds;
			if (_Odds == 0)
			{
				_OddsAgainst = false;
				_Odds = 1;
			}
		}

		public static bool TreatStackAsArmored(
			IEnumerable<SingleAttackOrder> Attackers,
			IEnumerable<Unit> Defenders)
		{
			var armoredCount = Defenders.Count(i => i.Configuration.IsArmored);
			var unArmoredCount = Defenders.Count(i => !i.Configuration.IsArmored);
			if (armoredCount > unArmoredCount) return true;
			if (armoredCount < unArmoredCount) return false;

			foreach (SingleAttackOrder a in Attackers) a.TreatStackAsArmored = true;
			var armoredAttack = Attackers.Sum(
				i => i.GetAttack().Attack);

			foreach (SingleAttackOrder a in Attackers) a.TreatStackAsArmored = false;
			var unArmoredAttack = Attackers.Sum(
				i => i.GetAttack().Attack);

			return armoredAttack < unArmoredAttack;
		}

		public int CompareTo(object Object)
		{
			if (!(Object is OddsCalculation)) return 1;

			var o = (OddsCalculation)Object;
			return (int)Math.Ceiling((1f * TotalAttack / TotalDefense) - (1f * o.TotalAttack / o.TotalDefense));
		}

		public override string ToString()
		{
			return string.Format(
				"[OddsCalculation: OddsAgainst={0}, Odds={1}, DieModifier={2}, TotalAttack={3}, TotalDefense={4}]",
				OddsAgainst,
				Odds,
				DieModifier,
				TotalAttack,
				TotalDefense);
		}
	}
}
