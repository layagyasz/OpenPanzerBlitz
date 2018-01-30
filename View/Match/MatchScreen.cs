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
		public EventHandler<EventArgs> OnFinishClicked;
		public EventHandler<ValuedEventArgs<UnitView>> OnUnitAdded;

		public readonly UnitConfigurationRenderer UnitRenderer;
		public readonly FactionRenderer FactionRenderer;

		EventBuffer<EventArgs> _EventBuffer = new EventBuffer<EventArgs>();

		MatchInfoDisplay _InfoDisplay = new MatchInfoDisplay();
		StackLayer _StackLayer = new StackLayer();
		Button _FinishButton = new Button("large-button") { DisplayedString = "Next Phase" };
		TableRow _TurnCounter = new TableRow("overlay-turn-counter");

		public IEnumerable<UnitView> UnitViews
		{
			get
			{
				return _StackLayer.UnitViews;
			}
		}

		public MatchScreen(
			Vector2f WindowSize, Match Match, TileRenderer TileRenderer, UnitConfigurationRenderer UnitRenderer, FactionRenderer FactionRenderer)
			: base(WindowSize, Match.Map, TileRenderer)
		{
			Match.OnStartPhase += _EventBuffer.Hook((s, e) => HandleNewTurn(s, (StartTurnComponentEventArgs)e)).Invoke;

			this.UnitRenderer = UnitRenderer;
			this.FactionRenderer = FactionRenderer;

			EventHandler<NewUnitEventArgs> addUnitHandler =
				_EventBuffer.Hook((s, e) => AddUnit(s, (NewUnitEventArgs)e)).Invoke;
			foreach (Army a in Match.Armies)
			{
				a.OnUnitAdded += addUnitHandler;
				foreach (Unit u in a.Units) AddUnit(u);
			}

			for (int i = 0; i < Match.Scenario.Turns; ++i)
				_TurnCounter.Add(new Checkbox("overlay-turn-counter-box") { Enabled = false });

			_FinishButton.Position = Size - _FinishButton.Size - new Vector2f(32, 32);
			_FinishButton.OnClick += HandleFinishClicked;
			_InfoDisplay.Position = _FinishButton.Position - new Vector2f(0, _InfoDisplay.Size.Y + 16);

			_Items.Add(_FinishButton);
			_Items.Add(_InfoDisplay);
			_Items.Add(_TurnCounter);
		}

		void HandleNewTurn(object Sender, StartTurnComponentEventArgs E)
		{
			SetTurn(E.Turn);
			_InfoDisplay.SetViewItem(new FactionView(E.Turn.TurnInfo.Army.Configuration.Faction, FactionRenderer, 80));
		}

		void AddUnit(object Sender, NewUnitEventArgs E)
		{
			AddUnit(E.Unit);
		}

		public void AddUnit(Unit Unit)
		{
			UnitView unitView = new UnitView(Unit, UnitRenderer, .625f, true);
			_StackLayer.AddUnitView(unitView);
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
		}

		public override void Update(
			MouseController MouseController,
			KeyController KeyController,
			int DeltaT,
			Transform Transform)
		{
			_EventBuffer.DispatchEvents();

			if (OnPulse != null) OnPulse(this, EventArgs.Empty);

			Camera.Update(MouseController, KeyController, DeltaT, PaneLayer.Any(i => i.Hover));
			Transform = Camera.GetTransform();

			MapView.Update(MouseController, KeyController, DeltaT, Transform);
			HighlightLayer.Update(MouseController, KeyController, DeltaT, Transform);
			_StackLayer.Update(MouseController, KeyController, DeltaT, Transform);

			foreach (Pod p in _Items) p.Update(MouseController, KeyController, DeltaT, Transform.Identity);
			PaneLayer.Update(MouseController, KeyController, DeltaT, Transform.Identity);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			Transform = Camera.GetTransform();

			Target.Draw(_Backdrop, PrimitiveType.Quads);
			MapView.Draw(Target, Transform);
			HighlightLayer.Draw(Target, Transform);
			_StackLayer.Draw(Target, Transform);

			foreach (Pod p in _Items) p.Draw(Target, Transform.Identity);
			PaneLayer.Draw(Target, Transform.Identity);
		}
	}
}
