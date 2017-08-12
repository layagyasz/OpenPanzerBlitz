using System;

using Cardamom.Interface;
using Cardamom.Network;

namespace PanzerBlitz
{
	public class MatchLobbyStateController : ProgramStateController
	{
		LobbyContext _Context;

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			_Context = (LobbyContext)ProgramStateContext;

			return new MatchLobbyScreen(ProgramContext.ScreenResolution, _Context.IsHost, _Context.Lobby);
		}
	}
}
