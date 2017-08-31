using System;

using Cardamom.Interface;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class RegisterPlayerStateController : ProgramStateController
	{
		RegisterPlayerController _Controller;

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			RegisterPlayerScreen screen = new RegisterPlayerScreen(ProgramContext.ScreenResolution);
			_Controller = new RegisterPlayerController(screen);
			_Controller.OnRegister += HandleRegister;
			screen.OnLogIn += HandleLogIn;
			return screen;
		}

		void HandleRegister(object Sender, ValuedEventArgs<PlayerContext> E)
		{
			OnProgramStateTransition(this, new ProgramStateTransitionEventArgs(ProgramState.ONLINE_LANDING, E.Value));
		}

		void HandleLogIn(object Sender, EventArgs E)
		{
			OnProgramStateTransition(this, new ProgramStateTransitionEventArgs(ProgramState.LOG_IN_PLAYER, null));
		}
	}
}
