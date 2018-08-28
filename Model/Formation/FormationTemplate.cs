using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public interface FormationTemplate
	{
		double ExpectedValue { get; }
		IEnumerable<Formation> Generate(Random Random);
	}
}
