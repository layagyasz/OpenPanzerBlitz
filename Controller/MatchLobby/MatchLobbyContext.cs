﻿using System;
using System.Linq;

using Cardamom.Network;

namespace PanzerBlitz
{
	public class MatchLobbyContext : NetworkContext
	{
		public readonly MatchLobby Lobby;
		public readonly Chat Chat;

		public MatchLobbyContext(TCPClient Client, MatchLobby Lobby, Chat Chat)
			: base(Client)
		{
			this.Lobby = Lobby;
			this.Chat = Chat;
		}

		public MatchLobbyContext(TCPServer Server, MatchLobby Lobby, Chat Chat)
			: base(Server)
		{
			this.Lobby = Lobby;
			this.Chat = Chat;
		}

		public MatchLobbyAdapter MakeMatchLobbyAdapter()
		{
			if (IsHost) return new LocalMatchLobbyAdapter(Lobby);
			return new RemoteMatchLobbyAdapter(Client);
		}

		public ChatAdapter MakeChatAdapter()
		{
			if (IsHost) return new LocalChatAdapter(Chat);
			return new RemoteChatAdapter(Client);
		}

		public MatchContext MakeMatchContext()
		{
			Match match = new Match(Lobby.Scenario);
			OrderSerializer serializer = new OrderSerializer(match.GetGameObjects());

			if (IsHost)
			{
				Server.MessageAdapter = new MatchMessageSerializer(serializer);
				Server.RPCHandler = new MatchRPCHandler(match);
				match.OnExecuteOrder += (sender, e) => Server.Broadcast(new ExecuteOrderRequest(e.Order, serializer));
				return new MatchContext(
					Server,
					match,
					serializer,
					match.Armies.First(i => i.Configuration == Lobby.GetPlayerArmy(GameData.Player)));
			}
			else
			{
				Client.MessageAdapter = new MatchMessageSerializer(serializer);
				Client.RPCHandler = new MatchRPCHandler(match);
				return new MatchContext(
					Client,
					match,
					serializer,
					match.Armies.First(i => i.Configuration == Lobby.GetPlayerArmy(GameData.Player)));
			}
		}
	}
}
