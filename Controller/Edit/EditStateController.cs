using System;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public class EditStateController : ProgramStateController
	{
		EditController _Controller;

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			EditScreen screen = new EditScreen(
				ProgramContext.ScreenResolution,
				new Map(11, 33),
				TileRenderer.SUMMER_STEPPE);
			_Controller = new EditController(screen);

			return screen;
		}
	}
}
