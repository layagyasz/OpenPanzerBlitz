using System;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public class EditStateController : ProgramStateController
	{
		EditController _Controller;

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			GameScreen screen = new GameScreen(
				ProgramContext.ScreenResolution,
				new MapView(new Map(11, 33), TileRenderer.SUMMER_STEPPE),
				new ArmyView[] { });
			_Controller = new EditController(screen);

			return screen;
		}
	}
}
