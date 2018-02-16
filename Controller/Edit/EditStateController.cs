using System.Linq;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public class EditStateController : ProgramStateController
	{
		EditController _Controller;

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			var screen = new EditScreen(
				ProgramContext.ScreenResolution,
				new Map(11, 33, null, new IdGenerator()),
				GameData.TileRenderers.Values.First());
			_Controller = new EditController(screen);

			return screen;
		}
	}
}
