using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class HumanGameController
	{
		GameScreen _GameScreen;
		Highlight _MovementHighlight = new Highlight();

		Dictionary<TurnComponent, Subcontroller> _Controllers;

		TextBox _InfoDisplay = new TextBox("info-display");

		public readonly Match Match;

		public HumanGameController(
			Match Match, UnitConfigurationRenderer Renderer, GameScreen GameScreen, KeyController KeyController)
		{
			this.Match = Match;
			_GameScreen = GameScreen;

			Button finishButton = new Button("large-button") { DisplayedString = "Finish" };
			finishButton.Position = GameScreen.Size - finishButton.Size - new Vector2f(32, 32);
			finishButton.OnClick += EndTurn;
			GameScreen.AddItem(finishButton);

			_InfoDisplay.Position = finishButton.Position - new Vector2f(0, _InfoDisplay.Size.Y + 16);
			GameScreen.AddItem(_InfoDisplay);

			_Controllers = new Dictionary<TurnComponent, Subcontroller>()
			{
				{ TurnComponent.DEPLOYMENT, new DeploymentController(Match, Renderer, GameScreen) },
				{ TurnComponent.ATTACK, new AttackController(Match, GameScreen) },
				{ TurnComponent.VEHICLE_COMBAT_MOVEMENT, new OverrunController(Match, GameScreen) },
				{ TurnComponent.VEHICLE_MOVEMENT, new MovementController(true, Match, GameScreen) },
				{ TurnComponent.CLOSE_ASSAULT, new CloseAssaultController(Match, GameScreen) },
				{ TurnComponent.NON_VEHICLE_MOVEMENT, new MovementController(false, Match, GameScreen) }
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
			foreach (Army a in Match.Armies) a.OnStartPhase += HandleTurn;
			KeyController.OnKeyPressed += OnKeyPressed;
			Match.Start();
		}

		private void HandleTurn(object sender, StartTurnComponentEventArgs e)
		{
			_InfoDisplay.DisplayedString =
				string.Format("{0}\n{1}", ((Army)sender).ArmyConfiguration.Faction.Name, e.TurnComponent);
			_Controllers[e.TurnComponent].Begin((Army)sender);
		}

		private void EndTurn(object sender, EventArgs e)
		{
			TurnComponent t = Match.CurrentPhase.Item2;
			if (Match.ExecuteOrder(new NextPhaseOrder())) _Controllers[t].End();
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

		private void OnKeyPressed(object sender, KeyPressedEventArgs E)
		{
			Tuple<Army, TurnComponent> phase = Match.CurrentPhase;
			_Controllers[phase.Item2].HandleKeyPress(E.Key);
		}
	}
}
