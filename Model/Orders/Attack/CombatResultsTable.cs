using System;
namespace PanzerBlitz
{
	public class CombatResultsTable
	{
		public static readonly CombatResultsTable STANDARD_CRT = new CombatResultsTable(
			new CombatResult[,] {
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
		});

		CombatResult[,] _CombatResults;

		public CombatResultsTable(CombatResult[,] CombatResults)
		{
			_CombatResults = CombatResults;
		}

		public CombatResult GetCombatResult(OddsCalculation Odds, int Roll)
		{
			return _CombatResults[Odds.GetOddsIndex(), Roll + Odds.DieModifier + 2];
		}

		public double[] GetCombatResultProbabilities(OddsCalculation Odds)
		{
			double[] p = new double[Enum.GetValues(typeof(CombatResult)).Length];
			int oddsIndex = Odds.GetOddsIndex();
			for (int i = 0; i < 6; ++i)
			{
				p[(int)GetCombatResult(Odds, i)] += 1f / 6;
			}
			return p;
		}
	}
}
