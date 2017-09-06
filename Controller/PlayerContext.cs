using System;
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

		public PlayerContext(TCPServer Server, Player Player)
			: base(Server)
		{
			this.Player = Player;
		}

		public MatchLobbyContext MakeLobbyContext()
		{
			if (IsHost)
			{
				MatchLobby lobby = new MatchLobby(GameData.Scenarios.First());
				lobby.ApplyAction(new AddPlayerAction(Player));
				Chat chat = new Chat();

				Server.MessageAdapter = new NonMatchMessageSerializer();
				Server.RPCHandler = new LobbyServerRPCHandler(lobby, chat);

				lobby.OnActionApplied += (sender, e) => Server.Broadcast(new ApplyLobbyActionRequest(e.Value));
				chat.OnActionApplied += (sender, e) => Server.Broadcast(new ApplyChatActionRequest(e.Value));

				return new MatchLobbyContext(Server, lobby, chat);
			}
			else
			{
				Chat chat = new Chat();
				Client.MessageAdapter = new NonMatchMessageSerializer();
				LobbyRPCHandler handler = new LobbyRPCHandler(chat);
				Client.RPCHandler = handler;

				if (((BooleanResponse)Client.Call(
					new ApplyLobbyActionRequest(new AddPlayerAction(Player))).Get()).Value)
				{
					MatchLobby lobby = ((GetLobbyResponse)Client.Call(new GetLobbyRequest()).Get()).Lobby;
					handler.SetLobby(lobby);
					return new MatchLobbyContext(Client, lobby, chat);
				}
				else return null;
			}
		}
	}
}
