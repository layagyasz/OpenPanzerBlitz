using System;
using System.Linq;

using Cardamom.Interface.Items;

namespace PanzerBlitz
{
	public class AttackPane : Pane
	{
		TextBox _TextBox = new TextBox("attack-text-box");
		public readonly AttackOrder Attack;

		public AttackPane(AttackOrder Attack)
			: base("attack-pane")
		{
			this.Attack = Attack;
			Add(_TextBox);
		}

		public void UpdateDescription()
		{
			_TextBox.DisplayedString = string.Join(
				"\n\n", Attack.OddsCalculations.Select(i => DescribeOddsCalculation(i)));
		}

		private string DescribeOddsCalculation(OddsCalculation OddsCalulation)
		{
			return string.Format(
				"Odds: {0}-1 {1}\n\n{2}\n\n{3}",
				OddsCalulation.Odds,
				OddsCalulation.OddsAgainst ? "against" : "for",
				string.Join("\n", OddsCalulation.AttackFactorCalculations.Select(
					i => DescribeAttackFactorCalculation(i.Item2))),
				string.Join("\n", OddsCalulation.OddsCalculationFactors));
		}

		private string DescribeAttackFactorCalculation(AttackFactorCalculation AttackFactorCalculation)
		{
			return string.Format(
				"Attack {0}\n{1}",
				AttackFactorCalculation.Attack,
				string.Join("\n", AttackFactorCalculation.Factors.Select(i => "\t" + i.ToString())));
		}
	}
}
