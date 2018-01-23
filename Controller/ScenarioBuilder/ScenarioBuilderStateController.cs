using System;
using System.Linq;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public class ScenarioBuilderStateController : PagedProgramStateController
	{
		ScenarioParameters _Parameters;
		ScenarioBuilderController _Controller;

		public ScenarioBuilderStateController() : base(ProgramState.LANDING) { }

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			_Parameters = new ScenarioParameters(1939, Front.EAST, GameData.Environments.Values.First());

			ScenarioBuilderScreen screen = new ScenarioBuilderScreen(ProgramContext.ScreenResolution, _Parameters);
			screen.OnMainMenuButtonClicked += HandleBack;

			_Controller =
				new ScenarioBuilderController(
					new ScenarioBuilder(new ScenarioParameters(0, Front.ALL, GameData.Environments.Values.First())),
					screen);

			return screen;
		}
	}
}
