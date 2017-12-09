using System;
using System.Collections.Generic;

using Cardamom.Interface;
using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class HumanMatchPlayerController : MatchPlayerController
	{
		public readonly MatchAdapter Match;
		public readonly HashSet<Army> AllowedArmies;
		public readonly UnitConfigurationRenderer Renderer;

		MatchScreen _MatchScreen;
		Dictionary<TurnComponent, Subcontroller> _Controllers;
		TurnInfo _CurrentTurn;
		Unit _SelectedUnit;
		Highlight _Highlight;

		KeyController _KeyController;
		EventBuffer<ValuedEventArgs<UnitView>> _NewUnitBuffer;

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
			MatchScreen MatchScreen,
			KeyController KeyController)
		{
			_NewUnitBuffer = new EventBuffer<ValuedEventArgs<UnitView>>(AddUnit);

			this.Match = Match;
			this.AllowedArmies = new HashSet<Army>(AllowedArmies);
			this.Renderer = Renderer;

			_MatchScreen = MatchScreen;
			_MatchScreen.OnFinishClicked += EndTurn;
			_MatchScreen.OnUnitAdded += _NewUnitBuffer.QueueEvent;
			_MatchScreen.OnPulse += (sender, e) => _NewUnitBuffer.DispatchEvents();

			_Controllers = new Dictionary<TurnComponent, Subcontroller>()
			{
				{ TurnComponent.DEPLOYMENT, new DeploymentController(this) },
				{ TurnComponent.ATTACK, new AttackController(this) },
				{ TurnComponent.VEHICLE_COMBAT_MOVEMENT, new OverrunController(this) },
				{ TurnComponent.VEHICLE_MOVEMENT, new MovementController(this, true) },
				{ TurnComponent.CLOSE_ASSAULT, new CloseAssaultController(this) },
				{ TurnComponent.NON_VEHICLE_MOVEMENT, new MovementController(this, false) }
			};

			foreach (TileView t in MatchScreen.MapView.TilesEnumerable)
			{
				t.OnClick += OnTileClick;
				t.OnRightClick += OnTileRightClick;
			}
			foreach (UnitView u in _MatchScreen.UnitViews)
			{
				AddUnit(u);
			}
			_KeyController = KeyController;
			KeyController.OnKeyPressed += OnKeyPressed;
		}

		void AddUnit(object Sender, ValuedEventArgs<UnitView> E)
		{
			AddUnit(E.Value);
		}

		void AddUnit(UnitView UnitView)
		{
			UnitView.OnClick += OnUnitClick;
			UnitView.OnRightClick += OnUnitRightClick;
		}

		public void DoTurn(Turn Turn)
		{
			TurnComponent t = AllowedArmies.Contains(Turn.TurnInfo.Army)
										   ? Turn.TurnInfo.TurnComponent : TurnComponent.SPECTATE;
			_MatchScreen.SetEnabled(AllowedArmies.Contains(Turn.TurnInfo.Army));
			if (_CurrentTurn.Army != null && _Controllers.ContainsKey(_CurrentTurn.TurnComponent))
			{
				_Controllers[_CurrentTurn.TurnComponent].End();
				UnHighlight();
				SelectUnit(null);
				_MatchScreen.PaneLayer.Clear();
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
			_MatchScreen.PaneLayer.Add(Pane);
		}

		public void RemovePane(Pane Pane)
		{
			_MatchScreen.PaneLayer.Remove(Pane);
		}

		public void SelectUnit(Unit Unit)
		{
			_SelectedUnit = Unit;
			if (Unit != null)
				_MatchScreen.InfoDisplay.SetViewItem(
					new UnitView(Unit, Renderer, 80, false) { Position = new Vector2f(40, 40) });
			else if (Match.GetTurn().TurnInfo.Army != null)
				_MatchScreen.InfoDisplay.SetViewItem(
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
			_MatchScreen.Alert(ObjectDescriber.Describe(Alert));
		}

		public void UnHighlight()
		{
			_MatchScreen.HighlightLayer.RemoveHighlight(_Highlight);
			_Highlight = new Highlight();
			_MatchScreen.HighlightLayer.AddHighlight(_Highlight);
		}

		public void Highlight(IEnumerable<Tuple<Tile, Color>> Highlight)
		{
			_MatchScreen.HighlightLayer.RemoveHighlight(_Highlight);
			_Highlight = new Highlight(Highlight);
			_MatchScreen.HighlightLayer.AddHighlight(_Highlight);
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
