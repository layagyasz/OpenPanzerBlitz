using System.Collections.Generic;
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

		public MatchLobbyContext(
			TCPServer Server, ConnectionCache<Player> ConnectionCache, MatchLobby Lobby, Chat Chat)
			: base(Server, ConnectionCache)
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

		public MatchContext MakeMatchContext(Scenario StaticScenario)
		{
			var match = new Match(StaticScenario, IsHost ? new FullOrderAutomater() : null);
			var serializer = new OrderSerializer(match);

			if (IsHost)
			{
				var armyConnections = new Dictionary<Army, Player>();
				foreach (Player p in Lobby.Players)
				{
					if (ConnectionCache.Connections.ContainsKey(p))
					{
						var a = match.Armies.FirstOrDefault(
							i => i.Configuration.UniqueKey == Lobby.GetPlayerArmy(p).UniqueKey);
						armyConnections.Add(a, p);
					}
				}

				Server.RPCHandler.WaitForPromises();
				Server.MessageAdapter = new MatchMessageSerializer(serializer);
				Server.RPCHandler =
					new RPCHandler().Install(new MatchServerLayer(match, armyConnections, ConnectionCache));
				match.OnExecuteOrder += (sender, e) => Server.Broadcast(new ExecuteOrderRequest(e.Order, serializer));
				return new MatchContext(
					Server,
					ConnectionCache,
					match,
					serializer,
					match.Armies.First(
						i => i.Configuration.UniqueKey == Lobby.GetPlayerArmy(GameData.Player).UniqueKey));
			}

			Client.MessageAdapter = new MatchMessageSerializer(serializer);
			Client.RPCHandler = new RPCHandler().Install(new MatchLayer(match));
			return new MatchContext(
				Client,
				match,
				serializer,
				match.Armies.First(
					i => i.Configuration.UniqueKey == Lobby.GetPlayerArmy(GameData.Player).UniqueKey));
		}
	}
}
