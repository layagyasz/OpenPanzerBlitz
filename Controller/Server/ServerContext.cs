using System;

using Cardamom.Network;

namespace PanzerBlitz
{
	public class ServerContext : NetworkContext
	{
		public readonly PanzerBlitzServer PanzerBlitzServer;

		public ServerContext(TCPServer Server)
			: base(Server)
		{
			PanzerBlitzServer = new PanzerBlitzServer(Server, "./BLKConfigurations/Server");
		}

		new public static ServerContext CreateServer(ushort Port)
		{
			TCPServer server = new TCPServer(Port);
			server.Start();
			return new ServerContext(server);
		}
	}
}
