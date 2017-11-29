using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class OddsCalculation
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
			Tile Tile)
		{
			OddsCalculationFactors = new List<OddsCalculationFactor>();

			StackArmored = Defenders.Any(i => i.Configuration.UnitClass == UnitClass.FORT)
					 			|| Tile.RulesCalculator.TreatUnitsAsArmored
								|| TreatStackAsArmored(AttackOrders, Defenders);
			// If there is a fort, only use its defense.
			Unit fort = Defenders.First().Position.Units.FirstOrDefault(
							i => i.Configuration.UnitClass == UnitClass.FORT && i.Army == Defenders.First().Army);
			if (fort != null)
			{
				TotalDefense = fort.Configuration.Defense;
				StackArmored = fort.Configuration.IsArmored;
				OddsCalculationFactors.Add(OddsCalculationFactor.FORT);
			}
			else TotalDefense = Defenders.Sum(i => i.Configuration.Defense);
			foreach (SingleAttackOrder a in AttackOrders) a.SetTreatStackAsArmored(StackArmored);

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

			// Terrain modifiers.
			if (Defenders.First().Position.RulesCalculator.DieModifier != 0)
			{
				_DieModifier += Defenders.First().Position.RulesCalculator.DieModifier;
				OddsCalculationFactors.Add(OddsCalculationFactor.TERRAIN);
			}

			// Disrupted modifier.
			if (Defenders.Any(i => i.Status == UnitStatus.DISRUPTED))
			{
				_DieModifier -= 1;
				OddsCalculationFactors.Add(OddsCalculationFactor.DISRUPTED);
			}

			// Clamp the die modifier.
			if (_DieModifier < -2) _DieModifier = -2;
			if (_DieModifier > 1) _DieModifier = 1;

			// Clamp the odds.
			if (_Odds > 4) _Odds = 4;
		}

		private void IncreaseOdds()
		{
			if (OddsAgainst) --_Odds;
			else ++_Odds;
			if (_Odds == 0)
			{
				_OddsAgainst = false;
				_Odds = 1;
			}
			if (_Odds > 4) _Odds = 4;
		}

		public static bool TreatStackAsArmored(
			IEnumerable<SingleAttackOrder> Attackers,
			IEnumerable<Unit> Defenders)
		{
			int armoredCount = Defenders.Count(i => i.Configuration.IsArmored);
			int unArmoredCount = Defenders.Count(i => !i.Configuration.IsArmored);
			if (armoredCount > unArmoredCount) return true;
			if (armoredCount < unArmoredCount) return false;

			foreach (SingleAttackOrder a in Attackers) a.SetTreatStackAsArmored(true);
			int armoredAttack = Attackers.Sum(
				i => i.GetAttack().Attack);

			foreach (SingleAttackOrder a in Attackers) a.SetTreatStackAsArmored(false);
			int unArmoredAttack = Attackers.Sum(
				i => i.GetAttack().Attack);

			return armoredAttack < unArmoredAttack;
		}
	}
}
