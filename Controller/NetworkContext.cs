using System;

using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class NetworkContext : ProgramStateContext
	{
		public readonly TCPClient Client;
		public readonly TCPServer Server;

		public bool IsHost
		{
			get
			{
				return Client == null;
			}
		}

		protected NetworkContext() { }

		protected NetworkContext(TCPClient Client)
		{
			this.Client = Client;
		}

		protected NetworkContext(TCPServer Server)
		{
			this.Server = Server;
		}

		public void Close()
		{
			if (Client != null) Client.Close();
			if (Server != null) Server.Close();
		}

		public MatchLobbyContext MakeLobbyContext()
		{
			if (IsHost)
			{
				MatchLobby lobby = new MatchLobby();
				lobby.ApplyAction(new AddPlayerAction(GameData.Player));
				Chat chat = new Chat();

				Server.MessageAdapter = new NonMatchMessageSerializer();
				Server.RPCHandler = new LobbyRPCHandler(lobby, chat);

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
					new ApplyLobbyActionRequest(new AddPlayerAction(GameData.Player))).Get()).Value)
				{
					MatchLobby lobby = ((GetLobbyResponse)Client.Call(new GetLobbyRequest()).Get()).Lobby;
					handler.SetLobby(lobby);
					return new MatchLobbyContext(Client, lobby, chat);
				}
				else return null;
			}
		}

		public static NetworkContext CreateClient(string IpAddress, ushort Port)
		{
			try
			{
				TCPClient client = new TCPClient(IpAddress, Port);
				client.Start();
				return new NetworkContext(client);
			}
			catch
			{
				return null;
			}
		}

		public static NetworkContext CreateServer(ushort Port)
		{
			try
			{
				TCPServer server = new TCPServer(Port);
				server.Start();
				return new NetworkContext(server);
			}
			catch
			{
				return null;
			}
		}
	}
}
