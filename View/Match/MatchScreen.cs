using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using Cence;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class MatchScreen : MapScreenBase
	{
		public EventHandler<EventArgs> OnFinishClicked;

		public readonly List<ArmyView> ArmyViews;
		public readonly MatchInfoDisplay InfoDisplay = new MatchInfoDisplay();

		StackLayer _StackLayer = new StackLayer();
		Button _FinishButton = new Button("large-button") { DisplayedString = "Next Phase" };

		public MatchScreen(Vector2f WindowSize, Map Map, TileRenderer TileRenderer, IEnumerable<ArmyView> ArmyViews)
			: base(WindowSize, Map, TileRenderer)
		{
			this.ArmyViews = ArmyViews.ToList();
			foreach (ArmyView a in this.ArmyViews)
			{
				_StackLayer.AddArmyView(a);
				a.OnNewUnitView += (sender, e) => _StackLayer.AddUnitView(e.UnitView);
			}

			_FinishButton.Position = Size - _FinishButton.Size - new Vector2f(32, 32);
			_FinishButton.OnClick += HandleFinishClicked;
			InfoDisplay.Position = _FinishButton.Position - new Vector2f(0, InfoDisplay.Size.Y + 16);
			_Items.Add(_FinishButton);
			_Items.Add(InfoDisplay);
		}

		public void SetEnabled(bool Enabled)
		{
			_FinishButton.Enabled = Enabled;
		}

		void HandleFinishClicked(object Sender, EventArgs E)
		{
			if (OnFinishClicked != null) OnFinishClicked(this, E);
		}

		public override void Update(
			MouseController MouseController,
			KeyController KeyController,
			int DeltaT,
			Transform Transform)
		{
			if (OnPulse != null) OnPulse(this, EventArgs.Empty);

			Camera.Update(MouseController, KeyController, DeltaT, PaneLayer.Any(i => i.Hover));
			Transform = Camera.GetTransform();

			MapView.Update(MouseController, KeyController, DeltaT, Transform);
			HighlightLayer.Update(MouseController, KeyController, DeltaT, Transform);
			_StackLayer.Update(MouseController, KeyController, DeltaT, Transform);

			foreach (Pod p in _Items) p.Update(MouseController, KeyController, DeltaT, Transform.Identity);
			_AlertText.Update(MouseController, KeyController, DeltaT, Transform.Identity);
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
			_AlertText.Draw(Target, Transform.Identity);
			PaneLayer.Draw(Target, Transform.Identity);
		}
	}
}
