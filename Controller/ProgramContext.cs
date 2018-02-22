using Cardamom.Interface;

using SFML.Window;

namespace PanzerBlitz
{
	public class ProgramContext
	{
		public readonly Vector2f ScreenResolution;
		public readonly KeyController KeyController;

		public ProgramContext(Vector2f ScreenResolution, KeyController KeyController)
		{
			this.ScreenResolution = ScreenResolution;
			this.KeyController = KeyController;
		}
	}
}
