using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class ScenarioBuilder
	{
		public readonly ScenarioParameters Parameters;

		List<ArmyBuilder> _Armies = new List<ArmyBuilder>();
		IdGenerator _IdGenerator = new IdGenerator();

		public ScenarioBuilder(ScenarioParameters Parameters)
		{
			this.Parameters = Parameters;
		}

		public void AddArmy(Faction Faction, uint Points)
		{
			_Armies.Add(new ArmyBuilder(_IdGenerator, new ArmyParameters(Faction, Points, Parameters)));
		}
	}
}
