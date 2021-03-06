﻿using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class ScenarioBuilder
	{
		public readonly ScenarioParameters Parameters;

		readonly List<ArmyBuilder> _Armies = new List<ArmyBuilder>();

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
			var armyConfigurations = _Armies.Select(i => i.BuildArmyConfiguration()).ToList();
			return new Scenario(
				"Custom Scenario",
				armyConfigurations,
				new TurnConfiguration(
					Parameters.Turns,
					new StaticSequence(
						armyConfigurations.Count, Enumerable.Range(0, armyConfigurations.Count).Cast<byte>()),
					new StaticSequence(
						armyConfigurations.Count, Enumerable.Range(0, armyConfigurations.Count).Cast<byte>())),
				Parameters.Setting.Environment,
				new RandomMapConfiguration(
					Parameters.MapSize.X, Parameters.MapSize.Y, Parameters.Setting),
				new ScenarioRules(Parameters.FogOfWar, false));
		}
	}
}
