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
	public class HumanMatchPlayerController : MatchPlayerController
	{
		MatchScreen _GameScreen;
		Highlight _MovementHighlight = new Highlight();

		Dictionary<TurnComponent, Subcontroller> _Controllers;

		public readonly MatchAdapter Match;
		public readonly HashSet<Army> AllowedArmies;

		TurnComponent _CurrentTurn;

		public HumanMatchPlayerController(
			MatchAdapter Match,
			IEnumerable<Army> AllowedArmies,
			UnitConfigurationRenderer Renderer,
			MatchScreen GameScreen,
			KeyController KeyController)
		{
			this.Match = Match;
			this.AllowedArmies = new HashSet<Army>(AllowedArmies);
			_GameScreen = GameScreen;

			_GameScreen.OnFinishClicked += EndTurn;

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
			TurnComponent t = AllowedArmies.Contains(TurnInfo.Army) ? TurnInfo.TurnComponent : TurnComponent.SPECTATE;
			_GameScreen.SetInfoMessage(string.Format("{0}\n{1}", TurnInfo.Army.Configuration.Faction.Name, t));
			if (_Controllers.ContainsKey(_CurrentTurn))
				_Controllers[_CurrentTurn].End();
			_CurrentTurn = t;
			if (_Controllers.ContainsKey(t))
				_Controllers[t].Begin(TurnInfo.Army);
		}

		void EndTurn(object sender, EventArgs e)
		{
			TurnComponent t = Match.GetTurnInfo().TurnComponent;
			if (_Controllers.ContainsKey(t) && _Controllers[t].Finish())
			{
				Match.ExecuteOrder(new NextPhaseOrder());
			}
		}

		void OnTileClick(object sender, MouseEventArgs e)
		{
			TurnInfo phase = Match.GetTurnInfo();
			if (AllowedArmies.Contains(phase.Army))
				_Controllers[phase.TurnComponent].HandleTileLeftClick(((TileView)sender).Tile);
		}

		void OnTileRightClick(object sender, MouseEventArgs e)
		{
			TurnInfo phase = Match.GetTurnInfo();
			if (AllowedArmies.Contains(phase.Army))
				_Controllers[phase.TurnComponent].HandleTileRightClick(((TileView)sender).Tile);
		}

		void OnUnitClick(object sender, MouseEventArgs e)
		{
			TurnInfo phase = Match.GetTurnInfo();
			if (AllowedArmies.Contains(phase.Army))
				_Controllers[phase.TurnComponent].HandleUnitLeftClick(((UnitView)sender).Unit);
		}

		void OnUnitRightClick(object sender, MouseEventArgs e)
		{
			TurnInfo phase = Match.GetTurnInfo();
			if (AllowedArmies.Contains(phase.Army))
				_Controllers[phase.TurnComponent].HandleUnitRightClick(((UnitView)sender).Unit);
		}

		void OnKeyPressed(object sender, KeyPressedEventArgs E)
		{
			TurnInfo phase = Match.GetTurnInfo();
			if (AllowedArmies.Contains(phase.Army))
				_Controllers[phase.TurnComponent].HandleKeyPress(E.Key);
		}
	}
}
