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
		public readonly UnitConfigurationRenderer Renderer;

		MatchScreen _GameScreen;
		Dictionary<TurnComponent, Subcontroller> _Controllers;
		TurnInfo _CurrentTurn;
		Unit _SelectedUnit;
		Highlight _Highlight;

		KeyController _KeyController;

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
			this.Renderer = Renderer;
			_GameScreen = GameScreen;
			_GameScreen.OnFinishClicked += EndTurn;

			_Controllers = new Dictionary<TurnComponent, Subcontroller>()
			{
				{ TurnComponent.DEPLOYMENT, new DeploymentController(this) },
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
			_KeyController = KeyController;
			KeyController.OnKeyPressed += OnKeyPressed;
		}

		public void DoTurn(Turn Turn)
		{
			TurnComponent t = AllowedArmies.Contains(Turn.TurnInfo.Army)
										   ? Turn.TurnInfo.TurnComponent : TurnComponent.SPECTATE;
			_GameScreen.SetTurn(Turn);
			_GameScreen.InfoDisplay.SetViewItem(new FactionView(Turn.TurnInfo.Army.Configuration.Faction, 80));
			_GameScreen.SetEnabled(AllowedArmies.Contains(Turn.TurnInfo.Army));
			if (_CurrentTurn.Army != null && _Controllers.ContainsKey(_CurrentTurn.TurnComponent))
			{
				_Controllers[_CurrentTurn.TurnComponent].End();
				UnHighlight();
				SelectUnit(null);
				_GameScreen.PaneLayer.Clear();
			}
			_CurrentTurn = Turn.TurnInfo;

			if (_Controllers.ContainsKey(t))
				_Controllers[t].Begin();
		}

		void EndTurn(object sender, EventArgs e)
		{
			TurnComponent t = Match.GetTurn().TurnInfo.TurnComponent;
			if (_Controllers.ContainsKey(t) && _Controllers[t].Finish())
				Match.ExecuteOrder(new NextPhaseOrder(Match.GetTurn().TurnInfo.Army));
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
					new UnitView(Unit, Renderer, 80, false) { Position = new Vector2f(40, 40) });
			else if (Match.GetTurn().TurnInfo.Army != null)
				_GameScreen.InfoDisplay.SetViewItem(
					new FactionView(Match.GetTurn().TurnInfo.Army.Configuration.Faction, 80));
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
			TurnInfo phase = Match.GetTurn().TurnInfo;
			if (AllowedArmies.Contains(phase.Army))
				_Controllers[phase.TurnComponent].HandleTileLeftClick(((TileView)sender).Tile);
		}

		void OnTileRightClick(object sender, MouseEventArgs e)
		{
			TurnInfo phase = Match.GetTurn().TurnInfo;
			if (AllowedArmies.Contains(phase.Army))
				_Controllers[phase.TurnComponent].HandleTileRightClick(((TileView)sender).Tile);
		}

		void OnUnitClick(object sender, MouseEventArgs e)
		{
			TurnInfo phase = Match.GetTurn().TurnInfo;
			if (AllowedArmies.Contains(phase.Army))
			{
				if (Keyboard.IsKeyPressed(Keyboard.Key.LShift))
					_Controllers[phase.TurnComponent].HandleUnitShiftLeftClick(((UnitView)sender).Unit);
				else _Controllers[phase.TurnComponent].HandleUnitLeftClick(((UnitView)sender).Unit);
			}
		}

		void OnUnitRightClick(object sender, MouseEventArgs e)
		{
			TurnInfo phase = Match.GetTurn().TurnInfo;
			if (AllowedArmies.Contains(phase.Army))
				_Controllers[phase.TurnComponent].HandleUnitRightClick(((UnitView)sender).Unit);
		}

		void OnKeyPressed(object sender, KeyPressedEventArgs E)
		{
			TurnInfo phase = Match.GetTurn().TurnInfo;
			if (AllowedArmies.Contains(phase.Army))
				_Controllers[phase.TurnComponent].HandleKeyPress(E.Key);
		}

		public void Unhook()
		{
			_KeyController.OnKeyPressed -= OnKeyPressed;
		}
	}
}
