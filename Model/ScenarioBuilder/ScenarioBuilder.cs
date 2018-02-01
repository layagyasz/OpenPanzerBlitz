using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class ScenarioBuilder
	{
		public readonly ScenarioParameters Parameters;

		List<ArmyBuilder> _Armies = new List<ArmyBuilder>();

		public IEnumerable<ArmyBuilder> Armies
		{
			get
			{
				return _Armies;
			}
		}

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
			this.Parameters.Copy(Parameters);
			return true;
		}

		public bool AddArmy(ArmyBuilder Builder)
		{
			_Armies.Add(Builder);
			return true;
		}

		public bool RemoveArmy(ArmyBuilder Builder)
		{
			return _Armies.Remove(Builder);
		}

		public Scenario BuildScenario()
		{
			return new Scenario(
				_Armies.Select(i => i.BuildArmyConfiguration()),
				Parameters.Turns,
				Parameters.Environment,
				new RandomMapConfiguration(Parameters.MapSize.X, Parameters.MapSize.Y).MakeStaticMap());
		}
	}
}
