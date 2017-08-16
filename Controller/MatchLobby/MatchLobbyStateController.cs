using System;

using Cardamom.Interface;
using Cardamom.Network;

namespace PanzerBlitz
{
	public class MatchLobbyStateController : ProgramStateController
	{
		MatchLobbyContext _Context;
		MatchLobbyController _Controller;
		ChatController _ChatController;

		public override Pod SetupState(ProgramContext ProgramContext, ProgramStateContext ProgramStateContext)
		{
			_Context = (MatchLobbyContext)ProgramStateContext;

			MatchLobbyScreen screen = new MatchLobbyScreen(
				ProgramContext.ScreenResolution, _Context.IsHost, _Context.Lobby, _Context.Chat, GameData.Player);
			_Controller = new MatchLobbyController(_Context.MakeMatchLobbyAdapter(), screen);
			_ChatController = new ChatController(_Context.MakeChatAdapter(), screen.ChatView, GameData.Player);
			return screen;
		}
	}
}
