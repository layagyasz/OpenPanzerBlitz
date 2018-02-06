using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class HumanMatchPlayerController : MatchPlayerController
	{
		public static readonly Color[] HIGHLIGHT_COLORS =
		{
			new Color(0, 255, 0, 120),
			new Color(255, 255, 0, 120),
			new Color(255, 128, 0, 120),
			new Color(255, 0, 0, 120)
		};

		public static readonly Color[] DIM_HIGHLIGHT_COLORS =
		{
			new Color(0, 255, 0, 60),
			new Color(255, 255, 0, 60),
			new Color(255, 128, 0, 60),
			new Color(255, 0, 0, 60)
		};

		enum HighlightToggle { ENEMY_SIGHT_FIELD, VICTORY_CONDITION_FIELD };

		public readonly MatchAdapter Match;
		public readonly HashSet<Army> AllowedArmies;
		public readonly UnitConfigurationRenderer UnitConfigurationRenderer;

		MatchScreen _MatchScreen;
		Dictionary<TurnComponent, Subcontroller> _Controllers;
		TurnInfo _CurrentTurn;
		Unit _SelectedUnit;

		Highlight _Highlight;
		bool[] _HighlightToggles = new bool[Enum.GetValues(typeof(HighlightToggle)).Length];

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
			UnitConfigurationRenderer UnitConfigurationRenderer,
			MatchScreen MatchScreen,
			KeyController KeyController)
		{
			_NewUnitBuffer = new EventBuffer<ValuedEventArgs<UnitView>>();

			this.Match = Match;
			this.AllowedArmies = new HashSet<Army>(AllowedArmies);
			this.UnitConfigurationRenderer = UnitConfigurationRenderer;

			_MatchScreen = MatchScreen;
			_MatchScreen.OnFinishClicked += EndTurn;
			_MatchScreen.OnUnitAdded += _NewUnitBuffer.Hook(AddUnit).Invoke;
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
				_MatchScreen.SetViewUnit(Unit);
			else if (Match.GetTurn().TurnInfo.Army != null)
				_MatchScreen.SetViewFaction(Match.GetTurn().TurnInfo.Army.Configuration.Faction);
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

			for (int i = 0; i < _HighlightToggles.Length; ++i) _HighlightToggles[i] = false;
		}

		public void Highlight(IEnumerable<Tuple<Tile, Color>> Highlight)
		{
			_MatchScreen.HighlightLayer.RemoveHighlight(_Highlight);
			_Highlight = new Highlight(Highlight);
			_MatchScreen.HighlightLayer.AddHighlight(_Highlight);

			for (int i = 0; i < _HighlightToggles.Length; ++i) _HighlightToggles[i] = false;
		}

		public Color GetRangeColor(
			LineOfSight LineOfSight, Unit Unit, bool DirectFire, AttackMethod AttackMethod = AttackMethod.NORMAL_FIRE)
		{
			return (DirectFire ? HIGHLIGHT_COLORS : DIM_HIGHLIGHT_COLORS)[
				PosterizeLineOfSight(LineOfSight, Unit, AttackMethod)];
		}

		public int PosterizeLineOfSight(
			LineOfSight LineOfSight, Unit Unit, AttackMethod AttackMethod = AttackMethod.NORMAL_FIRE)
		{
			return Math.Min(
				LineOfSight.Range * HIGHLIGHT_COLORS.Length / (Unit.Configuration.GetRange(AttackMethod) + 1),
				HIGHLIGHT_COLORS.Length - 1);
		}

		public Color GetTileColor(Tile Tile, byte ForTeam)
		{
			if (Tile.ControllingArmy == null) return HIGHLIGHT_COLORS[2];
			if (Tile.ControllingArmy.Configuration.Team == ForTeam) return HIGHLIGHT_COLORS.First();
			return HIGHLIGHT_COLORS.Last();
		}

		void HighlightEnemyFieldOfSight(byte Team)
		{
			if (_HighlightToggles[(int)HighlightToggle.ENEMY_SIGHT_FIELD]) UnHighlight();
			else
			{
				Highlight(Match.GetArmies()
					 .Where(i => i.Configuration.Team != Team)
					 .SelectMany(i => i.Units)
					 .SelectMany(i =>
								 i.GetFieldOfSight(AttackMethod.NORMAL_FIRE).Select(
									 j => new Tuple<Tile, int>(j.Item1.Final, PosterizeLineOfSight(j.Item1, i))))
					 .GroupBy(i => i.Item1)
						  .Select(i => new Tuple<Tile, Color>(i.Key, HIGHLIGHT_COLORS[i.Min(j => j.Item2)])));
				_HighlightToggles[(int)HighlightToggle.ENEMY_SIGHT_FIELD] = true;
			}
		}

		void HighlightVictoryConditionField(Army Army)
		{
			if (_HighlightToggles[(int)HighlightToggle.VICTORY_CONDITION_FIELD]) UnHighlight();
			else
			{
				Highlight(
					Army.Configuration.VictoryCondition.GetTiles(Match.GetMap())
					.Select(i => new Tuple<Tile, Color>(i, GetTileColor(i, Army.Configuration.Team))));
				_HighlightToggles[(int)HighlightToggle.VICTORY_CONDITION_FIELD] = true;
			}
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
			{
				if (E.Key == Keyboard.Key.S) HighlightEnemyFieldOfSight(phase.Army.Configuration.Team);
				else if (E.Key == Keyboard.Key.V) HighlightVictoryConditionField(phase.Army);
				else _Controllers[phase.TurnComponent].HandleKeyPress(E.Key);
			}
		}

		public void Unhook()
		{
			_KeyController.OnKeyPressed -= OnKeyPressed;
		}
	}
}
