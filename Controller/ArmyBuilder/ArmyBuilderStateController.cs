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
			Faction faction = GameData.Factions["russian"];
			ArmyParameters parameters =
				new ArmyParameters(20, 1945, Front.EAST, GameData.Environments["summer-steppe"]);
			ArmyBuilderScreen screen =
				new ArmyBuilderScreen(
					ProgramContext.ScreenResolution,
					GameData.UnitConfigurationLinks,
					parameters,
					faction,
					new UnitConfigurationRenderer(
						GameData.UnitConfigurationLinks.Where(
							i => i.Faction == faction && parameters.Matches(i)).Select(i => i.UnitConfiguration),
						GameData.UnitRenderDetails,
						1024,
						128,
						new Font("Compacta Std Regular.otf")));
			screen.OnMainMenuButtonClicked += HandleBack;
			return screen;
		}
	}
}
