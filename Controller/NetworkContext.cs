using System;

using Cardamom.Network;

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

		public NetworkContext(TCPClient Client)
		{
			this.Client = Client;
		}

		public NetworkContext(TCPServer Server)
		{
			this.Server = Server;
		}
	}
}
