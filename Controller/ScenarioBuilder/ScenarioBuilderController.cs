using System;
using System.Linq;

using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class ScenarioBuilderController
	{
		IdGenerator _IdGenerator = new IdGenerator();
		ScenarioBuilder _ScenarioBuilder;
		ScenarioBuilderScreen _Screen;

		public ScenarioBuilderController(ScenarioBuilder ScenarioBuilder, ScenarioBuilderScreen Screen)
		{
			_ScenarioBuilder = ScenarioBuilder;
			_Screen = Screen;
			_Screen.OnArmyAdded += HandleArmyAdded;
		}

		void HandleParametersChanged(object Sender, ValuedEventArgs<ScenarioParameters> E)
		{
			_ScenarioBuilder.Apply(new SetScenarioParametersAction(E.Value));
		}

		void HandleArmyAdded(object Sender, EventArgs E)
		{
			ArmyBuilder builder =
				new ArmyBuilder(
					_IdGenerator,
					new ArmyParameters(GameData.Factions.Values.First(), 0, 1, _ScenarioBuilder.Parameters));
			_Screen.AddArmyBuilder(builder);
		}
	}
}
