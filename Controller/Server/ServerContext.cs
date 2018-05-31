using Cardamom.Network;

namespace PanzerBlitz
{
	public class ServerContext : NetworkContext
	{
		public readonly PanzerBlitzServer PanzerBlitzServer;

		public ServerContext(
			TCPServer Server, ConnectionCache<Player> ConnectionCache, PanzerBlitzServer PanzerBlitzServer)
			: base(Server, ConnectionCache)
		{
			this.PanzerBlitzServer = PanzerBlitzServer;
		}

		new public static ServerContext CreateServer(ushort Port)
		{
			var pbServer =
				new PanzerBlitzServer(
					"./Modules/" + GameData.LoadedModule + "/Server",
					new SqlDatabase("127.0.0.1", "_panzerblitzonline", "root", "panzerblitzonline"));
			var server = new TCPServer(Port);
			server.Start();
			server.MessageAdapter = new NonMatchMessageSerializer();
			server.RPCHandler = new RPCHandler().Install(new PanzerBlitzServerLayer(pbServer));
			return new ServerContext(server, new ConnectionCache<Player>(), pbServer);
		}
	}
}
