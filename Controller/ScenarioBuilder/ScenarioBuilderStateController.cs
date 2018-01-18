using System;
using System.Linq;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public class ScenarioBuilderStateController : PagedProgramStateController
	{
		ScenarioBuilderController _Controller;

		public ScenarioBuilderStateController() : base(ProgramState.LANDING) { }

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			ScenarioBuilderScreen screen = new ScenarioBuilderScreen(ProgramContext.ScreenResolution);
			screen.OnMainMenuButtonClicked += HandleBack;

			_Controller =
				new ScenarioBuilderController(
					new ScenarioBuilder(new ScenarioParameters(0, Front.ALL, GameData.Environments.Values.First())),
					screen);

			return screen;
		}
	}
}
