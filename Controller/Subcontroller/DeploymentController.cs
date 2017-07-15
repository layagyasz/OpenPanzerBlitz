using System;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class DeploymentController : Subcontroller
	{
		DeploymentPane _DeploymentPane;
		Army _Army;
		Match _Match;
		GameScreen _GameScreen;

		UnitConfigurationRenderer _Renderer;

		public DeploymentController(Match Match, UnitConfigurationRenderer Renderer, GameScreen GameScreen)
		{
			_Match = Match;
			_GameScreen = GameScreen;
			_Renderer = Renderer;
		}

		public void Begin(Army Army)
		{
			_Army = Army;
			_DeploymentPane = new DeploymentPane(Army, _Renderer);
			_GameScreen.AddPane(_DeploymentPane);
		}

		public void End()
		{
			_GameScreen.RemovePane(_DeploymentPane);
		}

		public void HandleTileLeftClick(Tile Tile)
		{
			if (_DeploymentPane.Value != null)
			{
				Unit unit = _DeploymentPane.Peek();
				DeployOrder o = new DeployOrder(unit, Tile);
				if (_Match.ExecuteOrder(o)) _DeploymentPane.Remove(unit);
				else _GameScreen.Alert(o.Validate().ToString());
			}
		}

		public void HandleTileRightClick(Tile Tile)
		{
		}

		public void HandleUnitLeftClick(Unit Unit)
		{
		}

		public void HandleUnitRightClick(Unit Unit)
		{
			DeployOrder o = new DeployOrder(Unit, null);
			if (_Match.ExecuteOrder(o)) _DeploymentPane.Add(Unit);
		}

		public void HandleKeyPress(Keyboard.Key Key)
		{
		}
	}
}
