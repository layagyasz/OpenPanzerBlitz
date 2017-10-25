using System;

using Cardamom.Interface;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class LogInPlayerStateController : PagedProgramStateController
	{
		LogInPlayerController _Controller;

		public LogInPlayerStateController()
			: base(ProgramState.LANDING) { }

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			_Context = ProgramStateContext;

			LogInPlayerScreen screen = new LogInPlayerScreen(ProgramContext.ScreenResolution);
			_Controller = new LogInPlayerController(screen);
			_Controller.OnLogIn += HandleLogIn;
			screen.OnRegister += HandleRegister;
			screen.OnMainMenuButtonClicked += HandleBack;
			return screen;
		}

		void HandleLogIn(object Sender, ValuedEventArgs<PlayerContext> E)
		{
			OnProgramStateTransition(this, new ProgramStateTransitionEventArgs(ProgramState.ONLINE_LANDING, E.Value));
		}

		void HandleRegister(object Sender, EventArgs E)
		{
			OnProgramStateTransition(this, new ProgramStateTransitionEventArgs(ProgramState.REGISTER_PLAYER, null));
		}
	}
}
