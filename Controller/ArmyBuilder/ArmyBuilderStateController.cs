using System;
using System.Linq;

using Cardamom.Interface;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class ArmyBuilderStateController : PagedProgramStateController
	{
		public ArmyBuilderStateController() : base(ProgramState.LANDING) { }

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			Faction faction = GameData.Factions["finnish"];
			ScenarioParameters scenarioParameters =
				new ScenarioParameters(1939, Front.EAST, GameData.Environments["summer-steppe"]);
			ArmyParameters parameters = new ArmyParameters(faction, 1000, scenarioParameters);
			ArmyBuilderScreen screen =
				new ArmyBuilderScreen(
					ProgramContext.ScreenResolution,
					GameData.UnitConfigurationLinks,
					parameters,
					new UnitConfigurationRenderer(
						GameData.UnitConfigurationLinks.Where(
							i => parameters.Matches(i)).Select(i => i.UnitConfiguration),
						GameData.UnitRenderDetails,
						128,
						1024,
						new Font("Compacta Std Regular.otf")));
			screen.OnMainMenuButtonClicked += HandleBack;
			return screen;
		}
	}
}
