using System;
using System.Collections.Generic;

using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class LobbyServerRPCHandler : LobbyRPCHandler
	{
		public readonly Dictionary<Player, TCPConnection> PlayerConnections = new Dictionary<Player, TCPConnection>();

		public LobbyServerRPCHandler(MatchLobby Lobby, Chat Chat)
			: base(Lobby, Chat) { }

		bool PlayerMatches(Player Player, TCPConnection Connection)
		{
			return PlayerConnections.ContainsValue(Connection) && Connection == PlayerConnections[Player];
		}

		protected override RPCResponse ApplyLobbyAction(ApplyLobbyActionRequest Request, TCPConnection Connection)
		{
			if (Request.Action is AddPlayerAction)
			{
				if (PlayerConnections.ContainsValue(Connection)) return new BooleanResponse(false);
				BooleanResponse r = (BooleanResponse)base.ApplyLobbyAction(Request, Connection);
				if (r.Value) PlayerConnections.Add(Request.Action.Player, Connection);
				return r;
			}
			if (PlayerMatches(Request.Action.Player, Connection)) return base.ApplyLobbyAction(Request, Connection);
			return new BooleanResponse(false);
		}

		protected override RPCResponse ApplyChatAction(ApplyChatActionRequest Request, TCPConnection Connection)
		{
			if (PlayerMatches(Request.Action.Player, Connection)) return base.ApplyChatAction(Request, Connection);
			return new BooleanResponse(false);
		}
	}
}
