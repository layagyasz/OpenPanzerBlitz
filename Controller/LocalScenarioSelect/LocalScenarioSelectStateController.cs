using System;

using Cardamom.Interface;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class LocalScenarioSelectStateController : PagedProgramStateController
	{
		public LocalScenarioSelectStateController()
			: base(ProgramState.LANDING) { }

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			_Context = ProgramStateContext;

			var scenarioSelect =
				new ScenarioSelectScreen(ProgramContext.ScreenResolution, GameData.Scenarios);
			scenarioSelect.OnScenarioSelected += HandleStartScenario;
			scenarioSelect.OnMainMenuButtonClicked += HandleBack;

			return scenarioSelect;
		}

		void HandleStartScenario(object Sender, ValuedEventArgs<Scenario> E)
		{
			OnProgramStateTransition(
				this, new ProgramStateTransitionEventArgs(
					ProgramState.MATCH,
					new MatchContext(new Match(E.Value.MakeStatic(new Random()), new FullOrderAutomater()))));
		}
	}
}
