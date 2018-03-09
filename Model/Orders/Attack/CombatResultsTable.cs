using System;
namespace PanzerBlitz
{
	public abstract class CombatResultsTable
	{
		public static readonly CombatResultsTable STANDARD_CRT = new StandardCombatResultsTable();
		public static readonly CombatResultsTable AA_CRT = new AntiAirCombatResultsTable();

		static readonly CombatResult[,] STANDARD_CRT_RESULTS =
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
			}
		};

		class StandardCombatResultsTable : CombatResultsTable
		{
			public override CombatResult GetCombatResult(OddsCalculation Odds, int Roll)
			{
				return STANDARD_CRT_RESULTS[GetOddsIndex(Odds), Roll + Odds.DieModifier + 2];
			}

			// Map 4, 3, 2, 1, 2, 3, 4 => 0 -> 6
			int GetOddsIndex(OddsCalculation Odds)
			{
				if (Odds.OddsAgainst) return 4 - Odds.Odds;
				return 2 + Odds.Odds;
			}
		}

		static readonly CombatResult[,] AA_CRT_RESULTS =
		{
			{
				CombatResult.DAMAGE,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS
			},
			{
				CombatResult.DESTROY,
				CombatResult.DAMAGE,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS
			},
			{
				CombatResult.DESTROY,
				CombatResult.DAMAGE,
				CombatResult.DAMAGE,
				CombatResult.MISS,
				CombatResult.MISS,
				CombatResult.MISS
			},
			{
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DAMAGE,
				CombatResult.DAMAGE,
				CombatResult.MISS,
				CombatResult.MISS
			},
			{
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DAMAGE,
				CombatResult.DAMAGE,
				CombatResult.DAMAGE,
				CombatResult.MISS
			},
			{
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DAMAGE,
				CombatResult.DAMAGE,
				CombatResult.DAMAGE
			},
			{
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DESTROY,
				CombatResult.DAMAGE,
				CombatResult.DAMAGE
			}
		};

		class AntiAirCombatResultsTable : CombatResultsTable
		{
			public override CombatResult GetCombatResult(OddsCalculation Odds, int Roll)
			{
				return AA_CRT_RESULTS[GetOddsIndex(Odds), Roll];
			}

			// Map 4, 3, 2, 1, 2, 3, 4 => 0 -> 6
			int GetOddsIndex(OddsCalculation Odds)
			{
				if (Odds.TotalAttack > 36) return 6;
				if (Odds.TotalAttack > 26) return 5;
				if (Odds.TotalAttack > 20) return 4;
				if (Odds.TotalAttack > 16) return 3;
				if (Odds.TotalAttack > 12) return 2;
				if (Odds.TotalAttack > 8) return 1;
				return 0;
			}
		}

		public abstract CombatResult GetCombatResult(OddsCalculation Odds, int Roll);

		public double[] GetCombatResultProbabilities(OddsCalculation Odds)
		{
			double[] p = new double[Enum.GetValues(typeof(CombatResult)).Length];
			for (int i = 0; i < 6; ++i)
			{
				p[(int)GetCombatResult(Odds, i)] += 1f / 6;
			}
			return p;
		}
	}
}
