using System;

using Cardamom.Interface;
using Cardamom.Network;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class LandingStateController : ProgramStateController
	{
		LandingController _Controller;

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			LandingScreen screen = new LandingScreen(ProgramContext.ScreenResolution);
			screen.LocalMatchButton.OnClick += HandleLocalMatch;
			screen.HostMatchButton.OnClick += HandleHostMatch;
			screen.EditButton.OnClick += HandleEdit;
			screen.ServerButton.OnClick += HandleStartServer;

			_Controller = new LandingController(screen);
			_Controller.OnConnectionSetup += HandleJoinRemoteMatch;

			return screen;
		}

		void HandleLocalMatch(object Sender, EventArgs E)
		{
			OnProgramStateTransition(
				this, new ProgramStateTransitionEventArgs(ProgramState.LOCAL_SCENARIO_SELECT, null));
		}

		void HandleJoinRemoteMatch(object Sender, ValuedEventArgs<MatchLobbyContext> E)
		{
			OnProgramStateTransition(
				this,
				new ProgramStateTransitionEventArgs(
					ProgramState.MATCH_LOBBY, E.Value));
		}

		void HandleHostMatch(object Sender, EventArgs E)
		{
			NetworkContext context = NetworkContext.CreateServer(GameData.OnlinePort);
			if (context != null)
			{
				OnProgramStateTransition(
					this, new ProgramStateTransitionEventArgs(
						ProgramState.MATCH_LOBBY, context.MakeLobbyContext()));
			}
		}

		void HandleEdit(object Sender, EventArgs E)
		{
			OnProgramStateTransition(this, new ProgramStateTransitionEventArgs(ProgramState.EDIT, null));
		}

		void HandleStartServer(object Sender, EventArgs E)
		{
			ServerContext server = ServerContext.CreateServer(GameData.OnlinePort);
			if (server != null)
				OnProgramStateTransition(this, new ProgramStateTransitionEventArgs(ProgramState.SERVER, server));
		}
	}
}
