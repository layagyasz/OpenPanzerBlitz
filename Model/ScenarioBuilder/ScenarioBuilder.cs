using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class ScenarioBuilder
	{
		public readonly ScenarioParameters Parameters;

		List<ArmyBuilder> _Armies = new List<ArmyBuilder>();

		public ScenarioBuilder(ScenarioParameters Parameters)
		{
			this.Parameters = Parameters;
		}

		public bool Apply(ScenarioBuilderAction Action)
		{
			return Action.Apply(this);
		}

		public bool SetParameters(ScenarioParameters Parameters)
		{
			Parameters.Copy(Parameters);
			return true;
		}

		public void AddArmy(ArmyBuilder Builder)
		{
			_Armies.Add(Builder);
		}
	}
}
