using System;
using System.Collections.Generic;

using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class LobbyRPCHandler : RPCHandler
	{
		MatchLobby _Lobby;
		Chat _Chat;

		public LobbyRPCHandler(Chat Chat)
		{
			_Chat = Chat;
			RegisterRPC(typeof(ApplyLobbyActionRequest), i => ApplyLobbyAction((ApplyLobbyActionRequest)i));
			RegisterRPC(typeof(GetLobbyRequest), i => GetLobby((GetLobbyRequest)i));
			RegisterRPC(typeof(ApplyChatActionRequest), i => ApplyChatAction((ApplyChatActionRequest)i));
		}

		public void SetLobby(MatchLobby Lobby)
		{
			_Lobby = Lobby;
		}

		public LobbyRPCHandler(MatchLobby Lobby, Chat Chat)
			: this(Chat)
		{
			_Lobby = Lobby;
		}

		RPCResponse ApplyLobbyAction(ApplyLobbyActionRequest Request)
		{
			return new BooleanResponse(_Lobby != null && _Lobby.ApplyAction(Request.Action));
		}

		RPCResponse ApplyChatAction(ApplyChatActionRequest Request)
		{
			return new BooleanResponse(_Chat != null && _Chat.ApplyAction(Request.Action));
		}

		RPCResponse GetLobby(GetLobbyRequest Request)
		{
			return new GetLobbyResponse(_Lobby);
		}
	}
}
