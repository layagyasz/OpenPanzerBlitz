using System.Linq;

using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class PlayerContext : NetworkContext
	{
		public readonly Player Player;

		public PlayerContext(Player Player)
		{
			this.Player = Player;
		}

		public PlayerContext(TCPClient Client, Player Player)
			: base(Client)
		{
			this.Player = Player;
		}

		public PlayerContext(TCPServer Server, ConnectionCache<Player> ConnectionCache, Player Player)
			: base(Server, ConnectionCache)
		{
			this.Player = Player;
		}

		public MatchLobbyContext MakeLobbyContext()
		{
			if (IsHost)
			{
				var lobby = new MatchLobby(GameData.Scenarios.First());
				lobby.ApplyAction(new AddPlayerAction(Player));
				var chat = new Chat();

				Server.MessageAdapter = new NonMatchMessageSerializer();
				Server.RPCHandler =
					new RPCHandler().Install(
						new LobbyServerLayer(lobby, ConnectionCache), new ChatServerLayer(chat, ConnectionCache));

				lobby.OnActionApplied += (sender, e) => Server.Broadcast(new ApplyLobbyActionRequest(e.Value));
				chat.OnActionApplied += (sender, e) => Server.Broadcast(new ApplyChatActionRequest(e.Value));

				return new MatchLobbyContext(Server, ConnectionCache, lobby, chat);
			}
			else
			{
				var chat = new Chat();
				Client.MessageAdapter = new NonMatchMessageSerializer();
				var lobbyLayer = new LobbyLayer();
				var handler = new RPCHandler().Install(lobbyLayer, new ChatLayer(chat));
				Client.RPCHandler = handler;

				if (((BooleanResponse)Client.Call(
					new ApplyLobbyActionRequest(new AddPlayerAction(Player))).Get()).Value)
				{
					MatchLobby lobby = ((GetLobbyResponse)Client.Call(new GetLobbyRequest()).Get()).Lobby;
					lobbyLayer.Lobby = lobby;
					return new MatchLobbyContext(Client, lobby, chat);
				}
				return null;
			}
		}
	}
}
