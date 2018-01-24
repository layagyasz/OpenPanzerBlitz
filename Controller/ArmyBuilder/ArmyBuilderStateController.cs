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
			_Context = ProgramStateContext;
			ScenarioBuilderContext context = (ScenarioBuilderContext)_Context;

			ArmyBuilder builder = context.ScenarioBuilder.Armies.First(i => !i.Validate());
			ArmyBuilderScreen screen =
				new ArmyBuilderScreen(
					ProgramContext.ScreenResolution,
					GameData.UnitConfigurationLinks,
					builder.Parameters,
					new UnitConfigurationRenderer(
						GameData.UnitConfigurationLinks.Where(
							i => builder.Parameters.Matches(i)).Select(i => i.UnitConfiguration),
						GameData.UnitRenderDetails,
						128,
						1024,
						new Font("Compacta Std Regular.otf")));
			screen.OnMainMenuButtonClicked += HandleBack;
			return screen;
		}
	}
}
