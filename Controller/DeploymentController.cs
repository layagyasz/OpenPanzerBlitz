using System;
namespace PanzerBlitz
{
	public class DeploymentController : Controller
	{
		DeploymentPane _DeploymentPane;
		Army _Army;
		Match _Match;
		GameScreen _GameScreen;

		public DeploymentController(Match Match, GameScreen GameScreen)
		{
			_Match = Match;
			_GameScreen = GameScreen;
		}

		public void Begin(Army Army)
		{
			_Army = Army;
			_DeploymentPane = new DeploymentPane(Army);
			_GameScreen.AddPane(_DeploymentPane);
		}

		public void HandleTileLeftClick(Tile Tile)
		{
			if (_DeploymentPane.SelectedUnit != null)
			{
				DeployOrder o = new DeployOrder(_DeploymentPane.SelectedUnit, Tile);
				if (_Match.ExecuteOrder(o)) _DeploymentPane.Remove(_DeploymentPane.SelectedUnit);
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
	}
}
