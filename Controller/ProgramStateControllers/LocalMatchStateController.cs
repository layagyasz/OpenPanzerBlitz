using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class LocalMatchStateController : ProgramStateController
	{
		Match _Match;
		GameController _GameController;

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			_Match = new Match(((MatchContext)ProgramStateContext).Scenario);
			UnitConfigurationRenderer renderer = new UnitConfigurationRenderer(
				_Match.Scenario, 1024, 128, new Font("Compacta Std Regular.otf"));

			GameScreen screen = new GameScreen(
				ProgramContext.ScreenResolution,
				new MapView(_Match.Map, TileRenderer.SUMMER_STEPPE),
				_Match.Armies.Select(i => new ArmyView(i, renderer)));
			HumanGamePlayerController controller =
				new HumanGamePlayerController(
					new LocalMatchAdapter(_Match), renderer, screen, ProgramContext.KeyController);
			Dictionary<Army, GamePlayerController> playerControllers = new Dictionary<Army, GamePlayerController>();
			foreach (Army a in _Match.Armies) playerControllers.Add(a, controller);
			_GameController = new GameController(_Match, playerControllers);

			return screen;
		}
	}
}
