using System;

using Cardamom.Interface;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class LocalScenarioSelectStateController : ProgramStateController
	{
		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			ScenarioSelectScreen scenarioSelect =
				new ScenarioSelectScreen(ProgramContext.ScreenResolution, GameData.Scenarios);
			scenarioSelect.OnScenarioSelected += HandleStartScenario;

			return scenarioSelect;
		}

		void HandleStartScenario(object Sender, ValuedEventArgs<Scenario> E)
		{
			OnProgramStateTransition(
				this, new ProgramStateTransitionEventArgs(ProgramState.LOCAL_MATCH, new MatchContext(E.Value)));
		}
	}
}
