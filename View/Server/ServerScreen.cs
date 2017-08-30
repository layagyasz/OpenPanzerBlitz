using System;

using Cardamom.Interface;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class ServerScreen : ScreenBase
	{
		public ServerScreen(Vector2f WindowSize)
					: base(WindowSize) { }

		public override void Update(
					MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{ }
	}
}
