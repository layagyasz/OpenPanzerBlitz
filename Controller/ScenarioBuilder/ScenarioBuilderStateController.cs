using System;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public class ScenarioBuilderStateController : PagedProgramStateController
	{
		public ScenarioBuilderStateController() : base(ProgramState.LANDING) { }

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			ScenarioBuilderScreen screen = new ScenarioBuilderScreen(ProgramContext.ScreenResolution);
			screen.OnMainMenuButtonClicked += HandleBack;
			return screen;
		}
	}
}
