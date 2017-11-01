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
		public readonly MatchAdapter Match;
		public readonly HashSet<Army> AllowedArmies;

		MatchScreen _GameScreen;
		UnitConfigurationRenderer _Renderer;
		Dictionary<TurnComponent, Subcontroller> _Controllers;
		TurnInfo _CurrentTurn;
		Unit _SelectedUnit;
		Highlight _Highlight;

		public TurnInfo CurrentTurn
		{
			get
			{
				return _CurrentTurn;
			}
		}
		public Unit SelectedUnit
		{
			get
			{
				return _SelectedUnit;
			}
		}

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

			_Renderer = Renderer;

			_Controllers = new Dictionary<TurnComponent, Subcontroller>()
			{
				{ TurnComponent.DEPLOYMENT, new DeploymentController(this, Renderer) },
				{ TurnComponent.ATTACK, new AttackController(this) },
				{ TurnComponent.VEHICLE_COMBAT_MOVEMENT, new OverrunController(this) },
				{ TurnComponent.VEHICLE_MOVEMENT, new MovementController(this, true) },
				{ TurnComponent.CLOSE_ASSAULT, new CloseAssaultController(this) },
				{ TurnComponent.NON_VEHICLE_MOVEMENT, new MovementController(this, false) }
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
			_GameScreen.InfoDisplay.SetTurnInfo(TurnInfo);
			_GameScreen.InfoDisplay.SetViewItem(new FactionView(TurnInfo.Army.Configuration.Faction, 80));
			_GameScreen.SetEnabled(AllowedArmies.Contains(TurnInfo.Army));
			if (_CurrentTurn != null && _Controllers.ContainsKey(_CurrentTurn.TurnComponent))
			{
				_Controllers[_CurrentTurn.TurnComponent].End();
				UnHighlight();
				SelectUnit(null);
				_GameScreen.PaneLayer.Clear();
			}
			_CurrentTurn = TurnInfo;

			if (_Controllers.ContainsKey(t))
				_Controllers[t].Begin();
		}

		void EndTurn(object sender, EventArgs e)
		{
			TurnComponent t = Match.GetTurnInfo().TurnComponent;
			if (_Controllers.ContainsKey(t) && _Controllers[t].Finish())
				Match.ExecuteOrder(new NextPhaseOrder(Match.GetTurnInfo().Army));
		}

		public void AddPane(Pane Pane)
		{
			_GameScreen.PaneLayer.Add(Pane);
		}

		public void RemovePane(Pane Pane)
		{
			_GameScreen.PaneLayer.Remove(Pane);
		}

		public void SelectUnit(Unit Unit)
		{
			_SelectedUnit = Unit;
			if (Unit != null)
				_GameScreen.InfoDisplay.SetViewItem(
					new UnitView(Unit, _Renderer, 80, false) { Position = new Vector2f(40, 40) });
			else if (Match.GetTurnInfo() != null)
				_GameScreen.InfoDisplay.SetViewItem(
					new FactionView(Match.GetTurnInfo().Army.Configuration.Faction, 80));
		}

		public bool ExecuteOrderAndAlert(Order Order)
		{
			OrderInvalidReason r = Match.ExecuteOrder(Order);
			if (r != OrderInvalidReason.NONE) Alert(r);
			return r == OrderInvalidReason.NONE;
		}

		public void Alert(object Alert)
		{
			_GameScreen.Alert(ObjectDescriber.Describe(Alert));
		}

		public void UnHighlight()
		{
			_GameScreen.HighlightLayer.RemoveHighlight(_Highlight);
			_Highlight = new Highlight();
			_GameScreen.HighlightLayer.AddHighlight(_Highlight);
		}

		public void Highlight(IEnumerable<Tuple<Tile, Color>> Highlight)
		{
			_GameScreen.HighlightLayer.RemoveHighlight(_Highlight);
			_Highlight = new Highlight(Highlight);
			_GameScreen.HighlightLayer.AddHighlight(_Highlight);
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
