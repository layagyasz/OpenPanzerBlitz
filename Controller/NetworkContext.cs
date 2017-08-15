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

				Server.MessageAdapter = new NonGameMessageSerializer();
				Server.RPCHandler = new LobbyRPCHandler(lobby);

				lobby.OnActionApplied += (sender, e) => Server.Broadcast(new ApplyLobbyActionRequest(e.Value));

				return new MatchLobbyContext(Server, lobby);
			}
			else
			{
				Client.MessageAdapter = new NonGameMessageSerializer();
				LobbyRPCHandler handler = new LobbyRPCHandler();
				Client.RPCHandler = handler;

				if (((BooleanResponse)Client.Call(
					new ApplyLobbyActionRequest(new AddPlayerAction(GameData.Player))).Get()).Value)
				{
					MatchLobby lobby = ((GetLobbyResponse)Client.Call(new GetLobbyRequest()).Get()).Lobby;
					handler.SetLobby(lobby);
					return new MatchLobbyContext(Client, lobby);
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
