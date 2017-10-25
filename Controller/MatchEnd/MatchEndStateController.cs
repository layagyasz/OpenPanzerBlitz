using System;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public class MatchEndStateController : PagedProgramStateController
	{
		public MatchEndStateController()
			: base(ProgramState.LANDING) { }

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			_Context = ProgramStateContext;

			MatchContext context = (MatchContext)ProgramStateContext;
			MatchEndScreen screen = new MatchEndScreen(context.Match, ProgramContext.ScreenResolution);
			screen.OnMainMenuButtonClicked += HandleBack;
			return screen;
		}
	}
}
