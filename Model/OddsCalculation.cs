﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class OddsCalculation
	{
		public readonly IEnumerable<Tuple<Unit, AttackFactorCalculation>> AttackFactorCalculations;
		public readonly IEnumerable<Unit> Defenders;
		public readonly List<OddsCalculationFactor> OddsCalculationFactors;

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
			IEnumerable<Tuple<Unit, LineOfSight>> Attackers,
			IEnumerable<Unit> Defenders,
			AttackMethod AttackMethod,
			Tile Tile)
		{
			bool stackArmored = TreatStackAsArmored(Attackers, Defenders, AttackMethod);
			AttackFactorCalculations = Attackers.Select(
				i => new Tuple<Unit, AttackFactorCalculation>(
					i.Item1, i.Item1.UnitConfiguration.GetAttack(AttackMethod, stackArmored, i.Item2)));
			this.Defenders = Defenders;

			OddsCalculationFactors = new List<OddsCalculationFactor>();

			// Calculate Initial Odds
			int totalAttack = AttackFactorCalculations.Sum(i => i.Item2.Attack);
			int totalDefense = Defenders.Sum(i => i.UnitConfiguration.Defense);
			if (totalAttack > totalDefense)
			{
				_Odds = totalAttack / totalDefense;
			}
			else
			{
				_Odds = totalDefense / totalAttack + 1;
				_OddsAgainst = true;
			}

			_DieModifier = 0;

			// Modifiers for special attacks.
			if (AttackMethod == AttackMethod.CLOSE_ASSAULT)
			{
				_DieModifier -= 2;
				OddsCalculationFactors.Add(OddsCalculationFactor.CLOSE_ASSAULT);
				if (Attackers
					.Select(i => i.Item1)
					.GroupBy(i => i.Position)
					.Any(i => i.Count(j => j.UnitConfiguration.UnitClass == UnitClass.ENGINEER) > 0 && i.Count() > 1))
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
			if (Defenders.First().Position.TileConfiguration.DieModifier != 0)
			{
				_DieModifier += Defenders.First().Position.TileConfiguration.DieModifier;
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

		private static bool TreatStackAsArmored(
			IEnumerable<Tuple<Unit, LineOfSight>> Attackers,
			IEnumerable<Unit> Defenders,
			AttackMethod AttackMethod)
		{
			int armoredCount = Defenders.Count(i => i.UnitConfiguration.IsArmored);
			int unArmoredCount = Defenders.Count(i => !i.UnitConfiguration.IsArmored);
			if (armoredCount > unArmoredCount) return true;
			else if (armoredCount < unArmoredCount) return false;
			else
			{
				int armoredAttack = Attackers.Sum(
					i => i.Item1.UnitConfiguration.GetAttack(AttackMethod, true, i.Item2).Attack);
				int unArmoredAttack = Attackers.Sum(
					i => i.Item1.UnitConfiguration.GetAttack(AttackMethod, false, i.Item2).Attack);
				if (armoredAttack > unArmoredAttack) return false;
				else return true;
			}
		}
	}
}
