using System;
using System.Collections.Generic;

using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class LobbyRPCHandler : RPCHandler
	{
		MatchLobby _Lobby;

		public LobbyRPCHandler()
		{
			RegisterRPC(typeof(ApplyLobbyActionRequest), i => ApplyLobbyAction((ApplyLobbyActionRequest)i));
			RegisterRPC(typeof(GetLobbyRequest), i => GetLobby((GetLobbyRequest)i));
		}

		public void SetLobby(MatchLobby Lobby)
		{
			_Lobby = Lobby;
		}

		public LobbyRPCHandler(MatchLobby Lobby)
			: this()
		{
			_Lobby = Lobby;
		}

		RPCResponse ApplyLobbyAction(ApplyLobbyActionRequest Request)
		{
			return new BooleanResponse(_Lobby != null && _Lobby.ApplyAction(Request.Action));
		}

		RPCResponse GetLobby(GetLobbyRequest Request)
		{
			return new GetLobbyResponse(_Lobby);
		}
	}
}
