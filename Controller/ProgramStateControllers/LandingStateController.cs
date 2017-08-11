using System;

using Cardamom.Interface;
using Cardamom.Network;

namespace PanzerBlitz
{
	public class LandingStateController : ProgramStateController
	{
		LandingController _Controller;

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			LandingScreen screen = new LandingScreen(ProgramContext.ScreenResolution);
			screen.LocalMatchButton.OnClick += HandleLocalMatch;
			screen.EditButton.OnClick += HandleEdit;

			_Controller = new LandingController(screen);
			_Controller.OnConnectionSetup += HandleJoinRemoteMatch;

			return screen;
		}

		void HandleLocalMatch(object Sender, EventArgs E)
		{
			if (OnProgramStateTransition != null)
				OnProgramStateTransition(
					this, new ProgramStateTransitionEventArgs(ProgramState.LOCAL_SCENARIO_SELECT, null));
		}

		void HandleJoinRemoteMatch(object Sender, ValueChangedEventArgs<TCPClient> E)
		{
			if (OnProgramStateTransition != null)
				OnProgramStateTransition(
					this,
					new ProgramStateTransitionEventArgs(
						ProgramState.REMOTE_MATCH_LOBBY, new RemoteMatchContext(E.Value)));
		}

		void HandleEdit(object Sender, EventArgs E)
		{
			if (OnProgramStateTransition != null)
				OnProgramStateTransition(this, new ProgramStateTransitionEventArgs(ProgramState.EDIT, null));
		}
	}
}
