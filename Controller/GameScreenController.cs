using System;
using System.Collections.Generic;
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

		Dictionary<TurnComponent, Controller> _Controllers;

		public readonly Match Match;

		public GameScreenController(Match Match, UnitConfigurationRenderer Renderer, GameScreen GameScreen)
		{
			this.Match = Match;
			_GameScreen = GameScreen;

			_Controllers = new Dictionary<TurnComponent, Controller>()
			{
				{ TurnComponent.DEPLOYMENT, new DeploymentController(Match, Renderer, GameScreen) }
			};

			foreach (TileView t in GameScreen.MapView.TilesEnumerable)
			{
				t.OnClick += OnTileClick;
				t.OnRightClick += OnTileRightClick;
			}
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
			_Controllers[e.TurnComponent].Begin((Army)sender);
		}

		private void OnTileClick(object sender, MouseEventArgs e)
		{
			Tuple<Army, TurnComponent> phase = Match.CurrentPhase;
			_Controllers[phase.Item2].HandleTileLeftClick(((TileView)sender).Tile);
		}

		private void OnTileRightClick(object sender, MouseEventArgs e)
		{
			Tuple<Army, TurnComponent> phase = Match.CurrentPhase;
			_Controllers[phase.Item2].HandleTileRightClick(((TileView)sender).Tile);
		}

		private void OnUnitClick(object sender, MouseEventArgs e)
		{
			Tuple<Army, TurnComponent> phase = Match.CurrentPhase;
			_Controllers[phase.Item2].HandleUnitLeftClick(((UnitView)sender).Unit);
		}

		private void OnUnitRightClick(object sender, MouseEventArgs e)
		{
			Tuple<Army, TurnComponent> phase = Match.CurrentPhase;
			_Controllers[phase.Item2].HandleUnitRightClick(((UnitView)sender).Unit);
		}
	}
}
