using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class AttackFactorCalculation
	{
		public readonly int Attack;
		public readonly List<AttackFactorCalculationFactor> Factors;

		public AttackFactorCalculation(int Attack, List<AttackFactorCalculationFactor> Factors)
		{
			this.Attack = Attack;
			this.Factors = Factors;
		}
	}
}
