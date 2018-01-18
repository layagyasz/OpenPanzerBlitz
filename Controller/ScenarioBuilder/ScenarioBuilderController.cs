using System;

using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class ScenarioBuilderController
	{
		ScenarioBuilder _ScenarioBuilder;

		public ScenarioBuilderController(ScenarioBuilder ScenarioBuilder, ScenarioBuilderScreen Screen)
		{
			_ScenarioBuilder = ScenarioBuilder;
		}

		void HandleParametersChanged(object Sender, ValuedEventArgs<ScenarioParameters> E)
		{
			_ScenarioBuilder.Apply(new SetScenarioParametersAction(E.Value));
		}
	}
}
