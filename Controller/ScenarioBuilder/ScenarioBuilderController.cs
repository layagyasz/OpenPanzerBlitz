using System;
using System.Linq;

using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class ScenarioBuilderController
	{
		public EventHandler OnFinished;

		readonly IdGenerator _IdGenerator = new IdGenerator();
		readonly ScenarioBuilder _ScenarioBuilder;
		readonly ScenarioBuilderScreen _Screen;

		public ScenarioBuilderController(ScenarioBuilder ScenarioBuilder, ScenarioBuilderScreen Screen)
		{
			_ScenarioBuilder = ScenarioBuilder;
			_Screen = Screen;

			_Screen.OnParametersChanged = HandleParametersChanged;
			_Screen.OnArmyAdded += HandleArmyAdded;
			_Screen.OnArmyParametersChanged += HandleArmyParametersChanged;
			_Screen.OnArmyRemoved += HandleArmyRemoved;
			_Screen.OnFinished += HandleFinished;
		}

		void HandleParametersChanged(object Sender, ValuedEventArgs<ScenarioParameters> E)
		{
			_ScenarioBuilder.SetParameters(E.Value);
		}

		void HandleArmyAdded(object Sender, EventArgs E)
		{
			var builder =
				new ArmyBuilder(
					_IdGenerator,
					new ArmyParameters(GameData.Factions.Values.First(), 0, 1, _ScenarioBuilder.Parameters));
			_Screen.AddArmyBuilder(builder);
			_ScenarioBuilder.AddArmy(builder);
		}

		void HandleArmyParametersChanged(object Sender, ValuedEventArgs<Tuple<ArmyBuilder, ArmyParameters>> E)
		{
			E.Value.Item1.SetParameters(E.Value.Item2);
		}

		void HandleArmyRemoved(object Sender, ValuedEventArgs<ArmyBuilder> E)
		{
			_Screen.RemoveArmyBuilder(E.Value);
			_ScenarioBuilder.RemoveArmy(E.Value);
		}

		void HandleFinished(object Sender, EventArgs E)
		{
			if (!_Screen.Validate()) _Screen.Alert("Points must be a number greater than 0.");
			else if (_ScenarioBuilder.Armies.Select(i => i.Parameters.Team).Distinct().Count() < 2)
				_Screen.Alert("At least 2 teams required.");
			else if (OnFinished != null) OnFinished(this, EventArgs.Empty);
		}
	}
}
