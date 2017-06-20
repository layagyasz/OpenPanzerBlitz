using System;
namespace PanzerBlitz
{
	public class AttackController : Controller
	{
		Match _Match;
		GameScreen _GameScreen;
		UnitConfigurationRenderer _Renderer;

		public AttackController(Match Match, UnitConfigurationRenderer Renderer, GameScreen GameScreen)
		{
			_Match = Match;
			_GameScreen = GameScreen;
			_Renderer = Renderer;
		}

		public void Begin(Army Army)
		{
		}

		public void End()
		{
		}

		public void HandleTileLeftClick(Tile Tile)
		{
		}

		public void HandleTileRightClick(Tile Tile)
		{
		}

		public void HandleUnitLeftClick(Unit Unit)
		{
		}

		public void HandleUnitRightClick(Unit Unit)
		{
		}
	}
}
