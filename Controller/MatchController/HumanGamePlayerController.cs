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
	public class HumanGamePlayerController : GamePlayerController
	{
		GameScreen _GameScreen;
		Highlight _MovementHighlight = new Highlight();

		Dictionary<TurnComponent, Subcontroller> _Controllers;

		TextBox _InfoDisplay = new TextBox("info-display");

		public readonly MatchAdapter Match;

		public HumanGamePlayerController(
			MatchAdapter Match, UnitConfigurationRenderer Renderer, GameScreen GameScreen, KeyController KeyController)
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
			KeyController.OnKeyPressed += OnKeyPressed;
		}

		public void DoTurn(TurnInfo TurnInfo)
		{
			_InfoDisplay.DisplayedString =
				string.Format("{0}\n{1}", TurnInfo.Army.Configuration.Faction.Name, TurnInfo.TurnComponent);
			_Controllers[TurnInfo.TurnComponent].Begin(TurnInfo.Army);
		}

		public void ExecuteOrder(Order Order) { }

		void EndTurn(object sender, EventArgs e)
		{
			TurnComponent t = Match.GetTurnInfo().TurnComponent;
			if (_Controllers[t].Finish())
			{
				NextPhaseOrder order = new NextPhaseOrder();
				if (Match.ValidateOrder(order))
				{
					_Controllers[t].End();
					Match.ExecuteOrder(order);
				}
			}
		}

		void OnTileClick(object sender, MouseEventArgs e)
		{
			TurnInfo phase = Match.GetTurnInfo();
			_Controllers[phase.TurnComponent].HandleTileLeftClick(((TileView)sender).Tile);
		}

		void OnTileRightClick(object sender, MouseEventArgs e)
		{
			TurnInfo phase = Match.GetTurnInfo();
			_Controllers[phase.TurnComponent].HandleTileRightClick(((TileView)sender).Tile);
		}

		void OnUnitClick(object sender, MouseEventArgs e)
		{
			TurnInfo phase = Match.GetTurnInfo();
			_Controllers[phase.TurnComponent].HandleUnitLeftClick(((UnitView)sender).Unit);
		}

		void OnUnitRightClick(object sender, MouseEventArgs e)
		{
			TurnInfo phase = Match.GetTurnInfo();
			_Controllers[phase.TurnComponent].HandleUnitRightClick(((UnitView)sender).Unit);
		}

		void OnKeyPressed(object sender, KeyPressedEventArgs E)
		{
			TurnInfo phase = Match.GetTurnInfo();
			_Controllers[phase.TurnComponent].HandleKeyPress(E.Key);
		}
	}
}