using Cardamom.Network;

namespace PanzerBlitz
{
	public class NetworkContext : ProgramStateContext
	{
		public readonly TCPClient Client;
		public readonly TCPServer Server;
		public readonly ConnectionCache<Player> ConnectionCache;

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

		protected NetworkContext(TCPServer Server, ConnectionCache<Player> ConnectionCache)
		{
			this.Server = Server;
			this.ConnectionCache = ConnectionCache;
		}

		public void Close()
		{
			if (Client != null) Client.Close();
			if (Server != null) Server.Close();
		}

		public PlayerContext MakePlayerContext(Player Player)
		{
			if (Client != null) return new PlayerContext(Client, Player);
			if (Server != null) return new PlayerContext(Server, ConnectionCache, Player);
			return new PlayerContext(Player);
		}

		public PlayerContext MakeLoggedInPlayerContext(string Username, string Password)
		{
			Client.MessageAdapter = new NonMatchMessageSerializer();
			Client.RPCHandler = new RPCHandler();

			Player p = Client.Call(new LogInPlayerRequest(Username, Password)).Get<LogInPlayerResponse>().Player;
			if (p == null) return null;
			return new PlayerContext(Client, p);
		}

		public static NetworkContext CreateClient(string IpAddress, ushort Port)
		{
			var client = new TCPClient(IpAddress, Port);
			client.Start();
			return new NetworkContext(client);
		}

		public static NetworkContext CreateServer(ushort Port)
		{
			var server = new TCPServer(Port);
			server.Start();
			return new NetworkContext(server, new ConnectionCache<Player>());
		}
	}
}
