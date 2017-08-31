using System;

using Cardamom.Interface;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class LogInPlayerStateController : ProgramStateController
	{
		LogInPlayerController _Controller;

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			LogInPlayerScreen screen = new LogInPlayerScreen(ProgramContext.ScreenResolution);
			_Controller = new LogInPlayerController(screen);
			_Controller.OnLogIn += HandleLogIn;
			screen.OnRegister += HandleRegister;
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
