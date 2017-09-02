using System;

using Cardamom.Interface;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class Camera
	{
		Vector2f _Window;
		Vector2f _Center;
		float _Zoom;

		public Camera(Vector2f Window, Vector2f Center, float Zoom)
		{
			_Window = Window;
			_Center = Center;
			_Zoom = Zoom;
		}

		public Transform GetTransform()
		{
			Transform t = Transform.Identity;
			t.Translate(_Window * .5f);
			t.Scale(_Zoom, _Zoom);
			t.Translate(-_Center);

			return t;
		}

		public void Update(MouseController MouseController, KeyController KeyController, int DeltaT, bool Blocked)
		{
			float step = 1f / DeltaT;
			if (Keyboard.IsKeyPressed(Keyboard.Key.Left)) _Center += new Vector2f(-step, 0);
			if (Keyboard.IsKeyPressed(Keyboard.Key.Right)) _Center += new Vector2f(step, 0);
			if (Keyboard.IsKeyPressed(Keyboard.Key.Up)) _Center += new Vector2f(0, -step);
			if (Keyboard.IsKeyPressed(Keyboard.Key.Down)) _Center += new Vector2f(0, step);

			if (Keyboard.IsKeyPressed(Keyboard.Key.PageUp)) _Zoom += 5 * step;
			if (Keyboard.IsKeyPressed(Keyboard.Key.PageDown)) _Zoom -= 5 * step;

			if (!Blocked)
			{
				_Zoom += MouseController.WheelDelta * 20 * step;
				_Center -= MouseController.DragDelta / _Zoom;
			}
		}
	}
}
