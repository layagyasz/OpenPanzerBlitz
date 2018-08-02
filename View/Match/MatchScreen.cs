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
	public class MatchScreen : MapScreenBase
	{
		readonly static Color[] FOG_OF_WAR_MASKS =
		{
			new Color(0, 0, 0, 180),
			new Color(0, 0, 0, 120),
			new Color(0, 0, 0, 60),
			new Color(255, 255, 255, 0)
		};

		public EventHandler<EventArgs> OnFinishClicked;
		public EventHandler<ValuedEventArgs<UnitView>> OnUnitAdded;

		public readonly Button LoadButton = new Button("action-button") { DisplayedString = "(L)oad" };
		public readonly Button UnloadButton = new Button("action-button") { DisplayedString = "(U)nload" };

		public readonly Button FortifyButton = new Button("action-button") { DisplayedString = "(F)ortify" };
		public readonly Button AbandonButton = new Button("action-button") { DisplayedString = "(A)bandon" };

		public readonly Button DismountButton = new Button("action-button") { DisplayedString = "(D)ismount" };
		public readonly Button MountButton = new Button("action-button") { DisplayedString = "(M)ount" };

		public readonly Button EvacuateButton = new Button("action-button") { DisplayedString = "(E)vacuate" };
		public readonly Button ReconButton = new Button("action-button") { DisplayedString = "(R)econ" };

		public readonly Button ClearMinefieldButton =
			new Button("action-button") { DisplayedString = "Clear M(I)nefield" };
		public readonly Button EmplaceButton = new Button("action-button") { DisplayedString = "Em(P)lace" };

		public readonly UnitConfigurationRenderer UnitRenderer;
		public readonly FactionRenderer FactionRenderer;

		readonly HashSet<Army> _FollowedArmies;
		readonly EventBuffer<EventArgs> _EventBuffer = new EventBuffer<EventArgs>();

		bool _FogOfWar;
		SightFinder _SightFinder;
		Action<object, EventArgs> _FogOfWarHandler;

		VictoryConditionDisplay _VictoryConditionDisplay = new VictoryConditionDisplay();
		MatchInfoDisplay _InfoDisplay = new MatchInfoDisplay();
		readonly StackLayer _StackLayer = new StackLayer();
		readonly Button _FinishButton = new Button("large-button") { DisplayedString = "Next Phase" };
		readonly TableRow _TurnCounter = new TableRow("overlay-turn-counter");
		readonly SingleColumnTable _ActionDisplay = new SingleColumnTable("actions-display");

		public IEnumerable<UnitView> UnitViews
		{
			get
			{
				return _StackLayer.UnitViews;
			}
		}
		public IEnumerable<Button> ActionButtons
		{
			get
			{
				yield return LoadButton;
				yield return UnloadButton;
				yield return FortifyButton;
				yield return AbandonButton;
				yield return DismountButton;
				yield return MountButton;
				yield return EvacuateButton;
				yield return ReconButton;
				yield return ClearMinefieldButton;
			}
		}

		public MatchScreen(
			Vector2f WindowSize,
			Match Match,
			IEnumerable<Army> FollowedArmies,
			TileRenderer TileRenderer,
			UnitConfigurationRenderer UnitRenderer,
			FactionRenderer FactionRenderer)
			: base(WindowSize, Match.Map, TileRenderer)
		{
			Match.OnStartPhase += _EventBuffer.Hook<StartTurnComponentEventArgs>(HandleNewTurn).Invoke;

			this.UnitRenderer = UnitRenderer;
			this.FactionRenderer = FactionRenderer;
			_FollowedArmies = new HashSet<Army>(FollowedArmies);
			_FogOfWar = Match.Scenario.Rules.FogOfWar;
			_FogOfWarHandler = _EventBuffer.Hook<SightUpdatedEventArgs>(HandleSightUpdated);

			EventHandler<NewUnitEventArgs> addUnitHandler = _EventBuffer.Hook<NewUnitEventArgs>(AddUnit).Invoke;
			foreach (Army a in Match.Armies)
			{
				a.OnUnitAdded += addUnitHandler;
				foreach (Unit u in a.Units) AddUnit(u);
			}

			for (int i = 0; i < Match.Scenario.TurnConfiguration.Turns; ++i)
				_TurnCounter.Add(new Checkbox("overlay-turn-counter-box") { Enabled = false });

			_FinishButton.Position = Size - _FinishButton.Size - new Vector2f(32, 32);
			_FinishButton.OnClick += HandleFinishClicked;
			_InfoDisplay.Position = _FinishButton.Position - new Vector2f(0, _InfoDisplay.Size.Y + 16);
			_VictoryConditionDisplay.Position =
				_InfoDisplay.Position - new Vector2f(0, _VictoryConditionDisplay.Size.Y + 16);
			_ActionDisplay.Position = new Vector2f(WindowSize.X - _ActionDisplay.Size.X - 16, 16);

			_TransformedItems.Add(_StackLayer);

			_Items.Add(_FinishButton);
			_Items.Add(_InfoDisplay);
			_Items.Add(_VictoryConditionDisplay);
			_Items.Add(_TurnCounter);
			_Items.Add(_ActionDisplay);
		}

		void HandleNewTurn(object Sender, StartTurnComponentEventArgs E)
		{
			SetTurn(E.Turn);
			_InfoDisplay.SetViewItem(new FactionView(E.Turn.TurnInfo.Army.Configuration.Faction, FactionRenderer, 80));

			if (_FollowedArmies.Contains(E.Turn.TurnInfo.Army))
			{
				if (E.Turn.TurnInfo.TurnComponent == TurnComponent.WAIT && _FogOfWar) FogOver();
				else SetSightFinder(E.Turn.TurnInfo.Army.SightFinder);
			}
		}

		void SetSightFinder(SightFinder SightFinder)
		{
			if (_SightFinder != SightFinder)
			{
				if (_SightFinder != null) _SightFinder.OnSightUpdated -= _FogOfWarHandler.Invoke;
				SightFinder.OnSightUpdated += _FogOfWarHandler.Invoke;
				SetFogOfWar(SightFinder);
				_SightFinder = SightFinder;
			}

		}

		void FogOver()
		{
			_SightFinder = null;
			foreach (TileView t in MapView.TilesEnumerable) t.SetMask(FOG_OF_WAR_MASKS[0]);
			_StackLayer.RemoveAll();
		}

		void SetFogOfWar(SightFinder SightFinder)
		{
			_StackLayer.SetUnitVisibilities(SightFinder);
			if (_FogOfWar)
			{
				foreach (TileView t in MapView.TilesEnumerable)
					t.SetMask(FOG_OF_WAR_MASKS[FogIndex(t.Tile, SightFinder.GetTileSightLevel(t.Tile))]);
			}
		}

		void HandleSightUpdated(object Sender, SightUpdatedEventArgs E)
		{
			_StackLayer.UpdateUnitVisibilities(E.Unit, E.Movement, E.UnitDeltas);
			if (_FogOfWar)
			{
				foreach (var delta in E.TileDeltas)
					MapView.Tiles[delta.Item1.Coordinate.X, delta.Item1.Coordinate.Y]
						   .SetMask(FOG_OF_WAR_MASKS[FogIndex(delta.Item1, delta.Item2)]);
			}
		}

		int FogIndex(Tile Tile, TileSightLevel Level)
		{
			if (Level == TileSightLevel.NONE || Level == TileSightLevel.HARD_SPOTTED) return (int)Level;
			if (Tile.Rules.Concealing || Tile.Rules.LowProfileConcealing) return 1;
			return (int)Level + 1;
		}

		void AddUnit(object Sender, NewUnitEventArgs E)
		{
			AddUnit(E.Unit);
		}

		public void AddUnit(Unit Unit)
		{
			var unitView = new UnitView(Unit, UnitRenderer, .625f, true);
			_StackLayer.AddUnitView(unitView, _SightFinder);
			if (OnUnitAdded != null) OnUnitAdded(this, new ValuedEventArgs<UnitView>(unitView));
		}

		public void SetEnabled(bool Enabled)
		{
			_FinishButton.Enabled = Enabled;
		}

		public void SetViewUnit(Unit Unit)
		{
			_InfoDisplay.SetViewItem(new UnitView(Unit, UnitRenderer, 80, false) { Position = new Vector2f(40, 40) });
		}

		public void SetViewFaction(Faction Faction)
		{
			_InfoDisplay.SetViewItem(new FactionView(Faction, FactionRenderer, 80));
		}

		void HandleFinishClicked(object Sender, EventArgs E)
		{
			if (OnFinishClicked != null) OnFinishClicked(this, E);
		}

		public void SetTurn(Turn Turn)
		{
			int i = 0;
			foreach (ClassedGuiItem box in _TurnCounter)
			{
				((Checkbox)box).Value = i < Turn.TurnNumber;
				i++;
			}

			_InfoDisplay.SetTurn(Turn);
			_VictoryConditionDisplay.SetVictoryCondition(Turn.TurnInfo.Army.Configuration.VictoryCondition);
		}

		public void SetActions(Func<Button, bool> Selector)
		{
			_ActionDisplay.Clear();
			foreach (Button button in ActionButtons.Where(Selector)) _ActionDisplay.Add(button);
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			_EventBuffer.DispatchEvents();
			base.Update(MouseController, KeyController, DeltaT, Transform);
		}
	}
}
