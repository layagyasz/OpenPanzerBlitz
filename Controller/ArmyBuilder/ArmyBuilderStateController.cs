using System;
using System.Linq;

using Cardamom.Interface;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class ArmyBuilderStateController : ProgramStateController
	{
		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			Faction faction = GameData.Factions["russian"];
			ArmyBuilderScreen screen =
				new ArmyBuilderScreen(
					ProgramContext.ScreenResolution,
					GameData.UnitConfigurationLinks,
					new ArmyParameters(20, 1940, Front.WEST),
					faction,
					new UnitConfigurationRenderer(
						GameData.UnitConfigurationLinks.Where(
							i => i.Faction == faction).Select(i => i.UnitConfiguration),
						GameData.UnitRenderDetails,
						1024,
						128,
						new Font("Compacta Std Regular.otf")));
			return screen;
		}
	}
}
