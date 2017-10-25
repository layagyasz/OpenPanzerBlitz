using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public abstract class ScreenBase : Pod
	{
		public EventHandler OnMainMenuButtonClicked;

		static Color ORANGE = new Color(172, 107, 26);
		static Color DARK_GRAY = new Color(10, 10, 10);
		Vertex[] _Backdrop;

		Button _MainMenuButton;

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
				_MainMenuButton = new Button("normal-button") { DisplayedString = "Main Menu" };
				_MainMenuButton.Position = WindowSize - _MainMenuButton.Size - new Vector2f(32, 32);
				_MainMenuButton.OnClick += HandleMainMenuButtonClicked;
			}
		}

		void HandleMainMenuButtonClicked(object Sender, EventArgs E)
		{
			if (OnMainMenuButtonClicked != null) OnMainMenuButtonClicked(this, E);
		}

		public virtual void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			if (_MainMenuButton != null) _MainMenuButton.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public virtual void Draw(RenderTarget Target, Transform Transform)
		{
			Target.Draw(_Backdrop, PrimitiveType.Quads);
			if (_MainMenuButton != null) _MainMenuButton.Draw(Target, Transform);
		}
	}
}
