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

		void HandleJoinRemoteMatch(object Sender, ValuedEventArgs<LobbyContext> E)
		{
			if (OnProgramStateTransition != null)
				OnProgramStateTransition(
					this,
					new ProgramStateTransitionEventArgs(
						ProgramState.MATCH_LOBBY, E.Value));
		}

		void HandleHostMatch(object Sender, EventArgs E)
		{
			if (OnProgramStateTransition != null)
			{
				MatchLobby lobby = new MatchLobby();
				lobby.ApplyAction(new AddPlayerAction(GameData.Player));

				TCPServer server = new TCPServer(1000);
				server.MessageAdapter = new NonGameMessageSerializer();
				server.RPCHandler = new LobbyRPCHandler(lobby);
				server.Start();

				lobby.OnActionApplied += (sender, e) => server.Broadcast(new ApplyLobbyActionRequest(e.Value));

				OnProgramStateTransition(
					this, new ProgramStateTransitionEventArgs(
						ProgramState.MATCH_LOBBY, new LobbyContext(server, lobby)));
			}
		}

		void HandleEdit(object Sender, EventArgs E)
		{
			if (OnProgramStateTransition != null)
				OnProgramStateTransition(this, new ProgramStateTransitionEventArgs(ProgramState.EDIT, null));
		}
	}
}
