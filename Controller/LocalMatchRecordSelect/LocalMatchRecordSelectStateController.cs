using System;

using Cardamom.Interface;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class LocalMatchRecordSelectStateController : PagedProgramStateController
	{
		public LocalMatchRecordSelectStateController()
			: base(ProgramState.LANDING) { }

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			_Context = ProgramStateContext;

			var scenarioSelect =
				new MatchRecordSelectScreen(
					ProgramContext.ScreenResolution, string.Format("./MatchRecords/{0}", GameData.LoadedModule));
			scenarioSelect.OnMatchRecordSelected += HandleStartScenario;
			scenarioSelect.OnMainMenuButtonClicked += HandleBack;

			return scenarioSelect;
		}

		void HandleStartScenario(object Sender, ValuedEventArgs<MatchRecord> E)
		{
			var m = new MatchContext(E.Value.Match);
			var a = m.MakeMatchAdapter();
			var controller = new MatchRecordReplayPlayerController(a, E.Value);
			foreach (Army army in E.Value.Match.Armies) m.OverridePlayerController(army, controller);

			OnProgramStateTransition(this, new ProgramStateTransitionEventArgs(ProgramState.MATCH, m));
		}
	}
}
