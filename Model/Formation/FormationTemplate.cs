using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public interface FormationTemplate
	{
		double ExpectedValue { get; }
		bool Matches(ArmyParameters Parameters);
		IEnumerable<Formation> Generate(Random Random, ArmyParameters Parameters);
	}
}
