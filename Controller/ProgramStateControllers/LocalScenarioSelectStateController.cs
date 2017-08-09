using System;

using Cardamom.Interface;

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

		void HandleStartScenario(object Sender, ValueChangedEventArgs<Scenario> E)
		{
			if (OnProgramStateTransition != null)
				OnProgramStateTransition(
					this, new ProgramStateTransitionEventArgs(ProgramState.LOCAL_MATCH, new MatchContext(E.Value)));
		}
	}
}
