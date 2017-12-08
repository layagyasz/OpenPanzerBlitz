using System;
using System.Collections.Generic;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public abstract class ScreenBase : Pod
	{
		public EventHandler OnMainMenuButtonClicked;
		public EventHandler<EventArgs> OnPulse;

		static Color ORANGE = new Color(172, 107, 26);
		static Color DARK_GRAY = new Color(10, 10, 10);

		public readonly PaneLayer PaneLayer = new PaneLayer();

		Vertex[] _Backdrop;
		protected List<Pod> _Items = new List<Pod>();

		public ScreenBase(Vector2f WindowSize, bool HasMainMenuButton = true)
		{
			_Backdrop = new Vertex[]
			{
				new Vertex(new Vector2f(0, 0), ORANGE),
				new Vertex(new Vector2f(WindowSize.X, 0), ORANGE),
				new Vertex(new Vector2f(WindowSize.X, .33f * WindowSize.Y), ORANGE),
				new Vertex(new Vector2f(0, .33f * WindowSize.Y), ORANGE),

				new Vertex(new Vector2f(0, .33f * WindowSize.Y), DARK_GRAY),
				new Vertex(new Vector2f(WindowSize.X, .33f * WindowSize.Y), DARK_GRAY),
				new Vertex(WindowSize, DARK_GRAY),
				new Vertex(new Vector2f(0, WindowSize.Y), DARK_GRAY)
			};
			if (HasMainMenuButton)
			{
				Button mainMenuButton = new Button("normal-button") { DisplayedString = "Main Menu" };
				mainMenuButton.Position = WindowSize - mainMenuButton.Size - new Vector2f(32, 32);
				mainMenuButton.OnClick += HandleMainMenuButtonClicked;
				_Items.Add(mainMenuButton);
			}
		}

		void HandleMainMenuButtonClicked(object Sender, EventArgs E)
		{
			if (OnMainMenuButtonClicked != null) OnMainMenuButtonClicked(this, E);
		}

		public virtual void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			if (OnPulse != null) OnPulse(this, EventArgs.Empty);

			foreach (Pod item in _Items) item.Update(MouseController, KeyController, DeltaT, Transform);
			PaneLayer.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public virtual void Draw(RenderTarget Target, Transform Transform)
		{
			Target.Draw(_Backdrop, PrimitiveType.Quads);
			foreach (Pod item in _Items) item.Draw(Target, Transform);
			PaneLayer.Draw(Target, Transform);
		}
	}
}
