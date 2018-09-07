using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public interface FormationTemplate
	{
		double GetExpectedValue(FormationParameters Parameters);
		bool Matches(FormationParameters Parameters);
		IEnumerable<Formation> Generate(Random Random, FormationParameters Parameters);
	}
}
