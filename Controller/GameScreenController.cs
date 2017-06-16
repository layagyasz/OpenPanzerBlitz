using System;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Interface;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class GameScreenController
	{
		static readonly Color[] LOS_COLORS = new Color[] { new Color(255, 0, 0, 120), new Color(0, 255, 0, 120) };

		GameScreen _GameScreen;
		Highlight _MovementHighlight = new Highlight();

		DeploymentPane _DeploymentPane;

		public readonly Match Match;

		public GameScreenController(Match Match, GameScreen GameScreen)
		{
			this.Match = Match;

			_GameScreen = GameScreen;
			foreach (TileView t in GameScreen.MapView.TilesEnumerable) t.OnClick += OnTileClick;
			foreach (ArmyView a in GameScreen.ArmyViews)
			{
				foreach (UnitView u in a.UnitViews)
				{
					u.OnClick += OnUnitClick;
					u.OnRightClick += OnUnitRightClick;
				}
			}
			foreach (Army a in Match.Armies)
			{
				a.OnStartPhase += HandleTurn;
			}
			Match.Start();
		}

		private void HandleTurn(object sender, StartTurnComponentEventArgs e)
		{
			switch (e.TurnComponent)
			{
				case TurnComponent.DEPLOYMENT:
					DoDeployPhase((Army)sender);
					break;
				default:
					break;
			}
		}

		private void DoDeployPhase(Army Army)
		{
			_DeploymentPane = new DeploymentPane(Army);
			_GameScreen.AddPane(_DeploymentPane);
		}

		private void OnTileClick(object sender, MouseEventArgs e)
		{
			Tuple<Army, TurnComponent> phase = Match.GetCurrentPhase();
			if (_DeploymentPane != null
				&& phase.Item2 == TurnComponent.DEPLOYMENT
				&& (phase.Item1 == null || phase.Item1 == _DeploymentPane.Army))
				DeployUnit(_DeploymentPane.SelectedUnit, ((TileView)sender).Tile);
		}

		private void OnUnitClick(object sender, MouseEventArgs e)
		{
			Unit unit = ((UnitView)sender).Unit;
			_GameScreen.HighlightLayer.RemoveHighlight(_MovementHighlight);

			_MovementHighlight = new Highlight(
				unit.GetFieldOfSight()
				.Select(
					i => new Tuple<Tile, Color>(
						i.Final, LOS_COLORS[(int)Math.Min(1, i.Range * 2 / unit.UnitConfiguration.Range)])));
			_GameScreen.HighlightLayer.AddHighlight(_MovementHighlight);
		}

		private void OnUnitRightClick(object sender, MouseEventArgs e)
		{
			Unit unit = ((UnitView)sender).Unit;
			Tuple<Army, TurnComponent> phase = Match.GetCurrentPhase();
			if (_DeploymentPane != null
				&& phase.Item2 == TurnComponent.DEPLOYMENT
				&& (phase.Item1 == null || phase.Item1 == _DeploymentPane.Army))
			{
				DeployOrder o = new DeployOrder(unit, null);
				if (Match.ExecuteOrder(o)) _DeploymentPane.Add(unit);
			}

		}

		private void DeployUnit(Unit Unit, Tile Tile)
		{
			if (Unit != null)
			{
				DeployOrder o = new DeployOrder(Unit, Tile);
				if (Match.ExecuteOrder(o)) _DeploymentPane.Remove(Unit);
			}
		}
	}
}
