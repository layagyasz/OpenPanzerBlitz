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
			new Color(0, 255, 0, 60),
			new Color(255, 255, 0, 60),
			new Color(255, 128, 0, 60),
			new Color(255, 0, 0, 60)
		};

		public static readonly Color ACCENT_COLOR = new Color(0, 0, 255, 60);

		enum HighlightToggle { ENEMY_SIGHT_FIELD, VICTORY_CONDITION_FIELD };

		public readonly MatchAdapter Match;
		public readonly HashSet<Army> AllowedArmies;
		public readonly UnitConfigurationRenderer UnitConfigurationRenderer;

		MatchScreen _MatchScreen;
		Pane _Pane;
		Dictionary<TurnComponent, Subcontroller> _Controllers;
		Dictionary<Button, Func<bool>> _AllowedActions;
		TurnInfo _CurrentTurn;
		Unit _SelectedUnit;

		Highlight _Highlight;
		bool[] _HighlightToggles = new bool[Enum.GetValues(typeof(HighlightToggle)).Length];

		readonly KeyController _KeyController;
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
			_MatchScreen.OnUnitAdded += _NewUnitBuffer.Hook<ValuedEventArgs<UnitView>>(AddUnit).Invoke;
			_MatchScreen.OnPulse += (sender, e) => _NewUnitBuffer.DispatchEvents();

			_Controllers = new Dictionary<TurnComponent, Subcontroller>
			{
				{ TurnComponent.DEPLOYMENT, new DeploymentController(this) },
				{ TurnComponent.AIRCRAFT, new AircraftController(this) },
				{ TurnComponent.ANTI_AIRCRAFT, new AntiAircraftController(this) },
				{ TurnComponent.ARTILLERY, new ArtilleryController(this) },
				{ TurnComponent.ATTACK, new AttackController(this) },
				{ TurnComponent.VEHICLE_COMBAT_MOVEMENT, new OverrunController(this) },
				{ TurnComponent.VEHICLE_MOVEMENT, new MovementController(this, true) },
				{ TurnComponent.CLOSE_ASSAULT, new CloseAssaultController(this) },
				{ TurnComponent.NON_VEHICLE_MOVEMENT, new MovementController(this, false) },
				{ TurnComponent.RESET, new NoOpController(this) },
				{ TurnComponent.WAIT, new NoOpController(this) },
				{ TurnComponent.SPECTATE, new NoOpController(this) },
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

			MatchScreen.LoadButton.OnClick +=
				(sender, e) => LoadUnit(_CurrentTurn.TurnComponent != TurnComponent.DEPLOYMENT);
			MatchScreen.UnloadButton.OnClick += (sender, e) => UnloadUnit();
			MatchScreen.DismountButton.OnClick += (sender, e) => Dismount();
			MatchScreen.MountButton.OnClick += (sender, e) => Mount();
			MatchScreen.EvacuateButton.OnClick += (sender, e) => Evacuate();
			MatchScreen.ReconButton.OnClick += (sender, e) => Recon();
			MatchScreen.ClearMinefieldButton.OnClick += (sender, e) => ClearMinefield();
			MatchScreen.EmplaceButton.OnClick += (sender, e) => Emplace();

			_AllowedActions = new Dictionary<Button, Func<bool>>
			{
				{ MatchScreen.LoadButton, () => _Controllers[_CurrentTurn.TurnComponent].CanLoad() },
				{ MatchScreen.UnloadButton, () => _Controllers[_CurrentTurn.TurnComponent].CanUnload() },
				{ MatchScreen.DismountButton, () => _Controllers[_CurrentTurn.TurnComponent].CanDismount() },
				{ MatchScreen.MountButton, () => _Controllers[_CurrentTurn.TurnComponent].CanMount() },
				{ MatchScreen.EvacuateButton, () => _Controllers[_CurrentTurn.TurnComponent].CanEvacuate() },
				{ MatchScreen.ReconButton, () => _Controllers[_CurrentTurn.TurnComponent].CanRecon() },
				{
					MatchScreen.ClearMinefieldButton,
					() => _Controllers[_CurrentTurn.TurnComponent].CanClearMinefield()
				},
				{ MatchScreen.EmplaceButton, () => _Controllers[_CurrentTurn.TurnComponent].CanEmplace() }
			};
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
				Clear();
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
				ExecuteOrderAndAlert(new NextPhaseOrder(Match.GetTurn().TurnInfo));
		}

		public void SetPane(Pane Pane)
		{
			_Pane = Pane;
			AddPane(Pane);
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
			{
				_MatchScreen.SetViewUnit(Unit);
				_MatchScreen.SetActions(i => _AllowedActions[i]());
			}
			else if (Match.GetTurn().TurnInfo.Army != null)
			{
				_MatchScreen.SetViewFaction(Match.GetTurn().TurnInfo.Army.Configuration.Faction);
				_MatchScreen.SetActions(i => false);
			}
		}

		public bool ExecuteOrderAndAlert(Order Order)
		{
			var r = Match.ExecuteOrder(Order);
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
			LineOfSight LineOfSight, Unit Unit, AttackMethod AttackMethod)
		{
			return HIGHLIGHT_COLORS[PosterizeLineOfSight(LineOfSight, Unit, AttackMethod)];
		}

		public int PosterizeLineOfSight(
			LineOfSight LineOfSight, Unit Unit, AttackMethod AttackMethod)
		{
			return Math.Min(
				LineOfSight.Range * HIGHLIGHT_COLORS.Length / (Unit.Configuration.GetRange(AttackMethod, false) + 1),
				HIGHLIGHT_COLORS.Length - 1);
		}

		public Color GetTileColor(Tile Tile, byte ForTeam)
		{
			if (Tile.ControllingArmy == null) return HIGHLIGHT_COLORS[2];
			if (Tile.ControllingArmy.Configuration.Team == ForTeam) return HIGHLIGHT_COLORS.First();
			return HIGHLIGHT_COLORS.Last();
		}

		public bool FilterVisible(Tile Tile)
		{
			if (!Match.GetScenario().FogOfWar) return true;

			if (Tile.Rules.Concealing || Tile.Rules.LowProfileConcealing)
				return _CurrentTurn.Army.SightFinder.HasTileSightLevel(Tile, TileSightLevel.HARD_SPOTTED);
			return _CurrentTurn.Army.SightFinder.HasTileSightLevel(Tile, TileSightLevel.SIGHTED);
		}

		void HighlightEnemyFieldOfSight(byte Team)
		{
			if (_HighlightToggles[(int)HighlightToggle.ENEMY_SIGHT_FIELD]) UnHighlight();
			else
			{
				Highlight(Match.GetArmies()
					 .Where(i => i.Configuration.Team != Team)
					 .SelectMany(i => i.Units)
					 .SelectMany(
							  i => i.GetFieldOfSight(
								 AttackMethod.DIRECT_FIRE,
								 _CurrentTurn.Army.SightFinder.GetUnitVisibility(i).LastSeen).Select(
									 j => new Tuple<Tile, int>(
										 j.Final, PosterizeLineOfSight(j, i, AttackMethod.DIRECT_FIRE))))
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

		void HighlightScore(Func<Tile, double> Scorer)
		{
			Highlight(Match.GetMap().TilesEnumerable.Select(i => new Tuple<Tile, Color>(i, GetScoreColor(Scorer(i)))));
		}

		Func<Tile, double> ScoreByThreat()
		{
			if (_SelectedUnit == null) return i => 0;

			TileEvaluator e = new TileEvaluator(new AIRoot(Match, _CurrentTurn.Army));
			e.ReEvaluate();
			return i =>
			{
				return e.GetThreatRating(i, _SelectedUnit);
			};
		}

		double ScoreByPotential(Tile Tile)
		{
			if (_SelectedUnit == null) return 0;
			return new TileEvaluator(new AIRoot(Match, _CurrentTurn.Army)).GetPotentialRating(Tile, _SelectedUnit);
		}

		Func<Tile, double> ScoreByFavorability()
		{
			if (_SelectedUnit == null) return i => 0;

			TileEvaluator e = new TileEvaluator(new AIRoot(Match, _CurrentTurn.Army));
			e.ReEvaluate();
			return i =>
			{
				return e.GetTileFavorability(i, _SelectedUnit);
			};
		}

		Color GetScoreColor(double Score)
		{
			byte c = (byte)Math.Min(255, 255 * (2 - 2 / (1 + Math.Exp(-Score))));
			return new Color(255, c, c, 60);
		}

		public virtual void Clear()
		{
			if (_Pane != null)
			{
				RemovePane(_Pane);
				_Pane = null;
			}
			UnHighlight();
		}

		public IEnumerable<Direction> GetReconDirections()
		{
			return Enum.GetValues(typeof(Direction)).Cast<Direction>().Where(i => SelectedUnit.CanExitDirection(i));
		}

		public void Recon()
		{
			if (SelectedUnit == null) return;

			var directions = GetReconDirections().ToList();
			if (directions.Count == 1) ReconDirection(directions.First());
			else if (directions.Count > 1)
			{
				Clear();
				var pane = new SelectPane<Direction>("Recon", directions);
				pane.OnItemSelected += ReconDirection;
				SetPane(pane);
			}
		}

		void ReconDirection(object Sender, ValuedEventArgs<Direction> E)
		{
			ReconDirection(E.Value);
		}

		void ReconDirection(Direction Direction)
		{
			if (SelectedUnit != null)
			{
				var order = new ReconOrder(SelectedUnit, Direction);
				if (ExecuteOrderAndAlert(order))
				{
					SelectUnit(null);
					UnHighlight();
				}
			}
			Clear();
		}

		public IEnumerable<Direction> GetEvacuateDirections()
		{
			return Enum.GetValues(typeof(Direction)).Cast<Direction>().Where(i => SelectedUnit.CanExitDirection(i));
		}

		public void Evacuate()
		{
			if (SelectedUnit == null) return;

			var directions = GetEvacuateDirections().ToList();
			if (directions.Count == 1) EvacuateDirection(directions.First());
			else if (directions.Count > 1)
			{
				Clear();
				var pane = new SelectPane<Direction>("Evacuate", directions);
				pane.OnItemSelected += EvacuateDirection;
				SetPane(pane);
			}
		}

		void EvacuateDirection(object Sender, ValuedEventArgs<Direction> E)
		{
			EvacuateDirection(E.Value);
		}

		void EvacuateDirection(Direction Direction)
		{
			if (SelectedUnit != null)
			{
				var order = new EvacuateOrder(SelectedUnit, Direction);
				if (ExecuteOrderAndAlert(order))
				{
					SelectUnit(null);
					UnHighlight();
				}
			}
			Clear();
		}

		public void ClearMinefield()
		{
			if (SelectedUnit == null) return;

			var onMine =
				SelectedUnit.Position.Units.FirstOrDefault(
					i => i.Configuration.UnitClass == UnitClass.MINEFIELD);
			if (onMine != null) ClearMinefield(onMine);
			else
			{
				var mines =
					SelectedUnit.Position.Neighbors()
							   .SelectMany(i => i.Units)
							   .Where(i => i.Configuration.UnitClass == UnitClass.MINEFIELD);
				if (mines.Count() == 1) ClearMinefield(mines.First());
				else if (mines.Count() > 1)
				{
					Clear();
					var pane = new SelectPane<Unit>("Clear Minefield", mines);
					pane.OnItemSelected += ClearMinefield;
					SetPane(pane);
				}
			}
		}

		void ClearMinefield(object Sender, ValuedEventArgs<Unit> E)
		{
			ClearMinefield(E.Value);
		}

		void ClearMinefield(Unit Minefield)
		{
			if (SelectedUnit != null)
			{
				var order = new ClearMinefieldOrder(SelectedUnit, Minefield);
				if (ExecuteOrderAndAlert(order))
				{
					SelectUnit(null);
					UnHighlight();
				}
			}
			Clear();
		}

		public void Emplace()
		{
			if (SelectedUnit == null) return;

			var emplaceables =
				SelectedUnit.Position.Neighbors()
						   .SelectMany(i => i.Units)
						   .Where(i => i.Configuration.IsEmplaceable());
			if (emplaceables.Count() == 1) Emplace(emplaceables.First());
			else if (emplaceables.Count() > 1)
			{
				Clear();
				var pane = new SelectPane<Unit>("Emplace Unit", emplaceables);
				pane.OnItemSelected += Emplace;
				SetPane(pane);
			}
		}

		void Emplace(object Sender, ValuedEventArgs<Unit> E)
		{
			Emplace(E.Value);
		}

		void Emplace(Unit Target)
		{
			if (SelectedUnit != null)
			{
				var order = new EmplaceOrder(SelectedUnit, Target);
				if (ExecuteOrderAndAlert(order))
				{
					SelectUnit(null);
					UnHighlight();
				}
			}
			Clear();
		}

		public void LoadUnit(bool UseMovement)
		{
			if (SelectedUnit == null) return;

			var canLoad = SelectedUnit.Position.Units
									  .Where(i => SelectedUnit.CanLoad(i, UseMovement) == OrderInvalidReason.NONE)
									  .ToList();
			if (canLoad.Count == 1)
			{
				LoadUnit(canLoad.First());
			}
			else if (canLoad.Count > 1)
			{
				Clear();
				var pane = new SelectPane<Unit>("Load Unit", canLoad);
				pane.OnItemSelected += LoadUnit;
				SetPane(pane);
			}
		}

		void LoadUnit(object Sender, ValuedEventArgs<Unit> E)
		{
			LoadUnit(E.Value);
		}

		void LoadUnit(Unit Unit)
		{
			if (SelectedUnit != null)
			{
				var order = new LoadOrder(
					SelectedUnit, Unit, CurrentTurn.TurnComponent != TurnComponent.DEPLOYMENT);
				if (ExecuteOrderAndAlert(order) && order.UseMovement)
				{
					SelectUnit(null);
					UnHighlight();
				}
				else SelectUnit(SelectedUnit);
			}
			Clear();
		}

		public void UnloadUnit()
		{
			if (SelectedUnit == null) return;

			var order = new UnloadOrder(
				SelectedUnit, CurrentTurn.TurnComponent != TurnComponent.DEPLOYMENT);
			if (ExecuteOrderAndAlert(order) && order.UseMovement)
			{
				SelectUnit(null);
				UnHighlight();
			}
			else SelectUnit(SelectedUnit);
		}

		public void Mount()
		{
			if (SelectedUnit == null) return;

			var order = new MountOrder(SelectedUnit, CurrentTurn.TurnComponent != TurnComponent.DEPLOYMENT);
			if (ExecuteOrderAndAlert(order) && order.UseMovement)
			{
				SelectUnit(null);
				UnHighlight();
			}
			else SelectUnit(SelectedUnit);
		}

		public void Dismount()
		{
			if (SelectedUnit == null) return;
			var order = new DismountOrder(SelectedUnit, CurrentTurn.TurnComponent != TurnComponent.DEPLOYMENT);
			if (ExecuteOrderAndAlert(order) && order.UseMovement)
			{
				SelectUnit(null);
				UnHighlight();
			}
			else SelectUnit(SelectedUnit);
		}

		public bool UseSecondaryWeapon()
		{
			return Keyboard.IsKeyPressed(Keyboard.Key.LControl);
		}

		void OnTileClick(object sender, MouseEventArgs e)
		{
			Console.WriteLine(((TileView)sender).Tile.Coordinate);
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
			{
				if (Keyboard.IsKeyPressed(Keyboard.Key.LShift))
					_Controllers[phase.TurnComponent].HandleUnitShiftRightClick(((UnitView)sender).Unit);
				else _Controllers[phase.TurnComponent].HandleUnitRightClick(((UnitView)sender).Unit);
			}
		}

		void OnKeyPressed(object sender, KeyPressedEventArgs E)
		{
			TurnInfo phase = Match.GetTurn().TurnInfo;
			if (AllowedArmies.Contains(phase.Army))
			{
				if (E.Key == Keyboard.Key.S) HighlightEnemyFieldOfSight(phase.Army.Configuration.Team);
				else if (E.Key == Keyboard.Key.V) HighlightVictoryConditionField(phase.Army);
				else if (E.Key == Keyboard.Key.Num1) HighlightScore(ScoreByThreat());
				else if (E.Key == Keyboard.Key.Num2) HighlightScore(ScoreByPotential);
				else if (E.Key == Keyboard.Key.Num3) HighlightScore(ScoreByFavorability());
				else _Controllers[phase.TurnComponent].HandleKeyPress(E.Key);
			}
		}

		public void Unhook()
		{
			_KeyController.OnKeyPressed -= OnKeyPressed;
		}
	}
}
