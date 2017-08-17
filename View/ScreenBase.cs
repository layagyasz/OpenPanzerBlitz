using System;

using Cardamom.Interface;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public abstract class ScreenBase : Pod
	{
		static Color ORANGE = new Color(172, 107, 26);
		static Color DARK_GRAY = new Color(10, 10, 10);
		Vertex[] _Backdrop;

		public ScreenBase(Vector2f WindowSize)
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
		}

		public abstract void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform);

		public virtual void Draw(RenderTarget Target, Transform Transform)
		{
			Target.Draw(_Backdrop, PrimitiveType.Quads);
		}
	}
}
