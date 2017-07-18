using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class OddsCalculation
	{
		public readonly IEnumerable<Tuple<SingleAttackOrder, AttackFactorCalculation>> AttackFactorCalculations;
		public readonly IEnumerable<Unit> Defenders;
		public readonly List<OddsCalculationFactor> OddsCalculationFactors;
		public readonly bool StackArmored;
		public readonly int TotalAttack;
		public readonly int TotalDefense;

		private bool _OddsAgainst;
		private int _Odds;
		private int _DieModifier;

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
			StackArmored = Tile.MovementProfile.TreatUnitsAsArmored
							   || TreatStackAsArmored(AttackOrders, Defenders, AttackMethod);
			foreach (SingleAttackOrder a in AttackOrders) a.SetTreatStackAsArmored(StackArmored);
			AttackFactorCalculations = AttackOrders.Select(
				i => new Tuple<SingleAttackOrder, AttackFactorCalculation>(
					i, i.GetAttack()));
			this.Defenders = Defenders;

			OddsCalculationFactors = new List<OddsCalculationFactor>();

			// Calculate Initial Odds
			TotalAttack = AttackFactorCalculations.Sum(i => i.Item2.Attack);
			TotalDefense = Defenders.Sum(i => i.UnitConfiguration.Defense);
			if (TotalAttack > TotalDefense)
			{
				_Odds = TotalAttack / TotalDefense;
			}
			else
			{
				_Odds = TotalDefense / TotalAttack + 1;
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
					.Any(i => i.Count(j => j.UnitConfiguration.IsEngineer) > 0 && i.Count() > 1))
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
			if (Defenders.First().Position.MovementProfile.DieModifier != 0)
			{
				_DieModifier += Defenders.First().Position.MovementProfile.DieModifier;
				OddsCalculationFactors.Add(OddsCalculationFactor.TERRAIN);
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
			IEnumerable<Unit> Defenders,
			AttackMethod AttackMethod)
		{
			int armoredCount = Defenders.Count(i => i.UnitConfiguration.IsArmored);
			int unArmoredCount = Defenders.Count(i => !i.UnitConfiguration.IsArmored);
			if (armoredCount > unArmoredCount) return true;
			else if (armoredCount < unArmoredCount) return false;
			else
			{
				foreach (SingleAttackOrder a in Attackers) a.SetTreatStackAsArmored(true);
				int armoredAttack = Attackers.Sum(
					i => i.GetAttack().Attack);

				foreach (SingleAttackOrder a in Attackers) a.SetTreatStackAsArmored(false);
				int unArmoredAttack = Attackers.Sum(
					i => i.GetAttack().Attack);

				if (armoredAttack > unArmoredAttack) return false;
				else return true;
			}
		}
	}
}
