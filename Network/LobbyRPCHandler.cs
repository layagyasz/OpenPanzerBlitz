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
			RegisterRPC(typeof(ApplyLobbyActionRequest), (i, j) => ApplyLobbyAction((ApplyLobbyActionRequest)i, j));
			RegisterRPC(typeof(GetLobbyRequest), (i, j) => GetLobby((GetLobbyRequest)i, j));
			RegisterRPC(typeof(ApplyChatActionRequest), (i, j) => ApplyChatAction((ApplyChatActionRequest)i, j));
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

		protected virtual RPCResponse ApplyLobbyAction(ApplyLobbyActionRequest Request, TCPConnection Connection)
		{
			return new BooleanResponse(_Lobby != null && _Lobby.ApplyAction(Request.Action));
		}

		protected virtual RPCResponse ApplyChatAction(ApplyChatActionRequest Request, TCPConnection Connection)
		{
			return new BooleanResponse(_Chat != null && _Chat.ApplyAction(Request.Action));
		}

		protected virtual RPCResponse GetLobby(GetLobbyRequest Request, TCPConnection Connection)
		{
			return new GetLobbyResponse(_Lobby);
		}
	}
}
