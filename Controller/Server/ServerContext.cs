using Cardamom.Network;

namespace PanzerBlitz
{
	public class ServerContext : NetworkContext
	{
		public readonly PanzerBlitzServer PanzerBlitzServer;

		public ServerContext(TCPServer Server, PanzerBlitzServer PanzerBlitzServer)
			: base(Server)
		{
			this.PanzerBlitzServer = PanzerBlitzServer;
		}

		new public static ServerContext CreateServer(ushort Port)
		{
			var pbServer = new PanzerBlitzServer("./" + GameData.LoadedModule + "/Server");
			var server = new TCPServer(Port);
			server.Start();
			server.MessageAdapter = new NonMatchMessageSerializer();
			server.RPCHandler = new PanzerBlitzServerRPCHandler(pbServer);
			return new ServerContext(server, pbServer);
		}
	}
}
