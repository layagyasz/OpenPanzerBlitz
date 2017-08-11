using System;

using Cardamom.Network;

namespace PanzerBlitz
{
	public class RemoteMatchContext : ProgramStateContext
	{
		public readonly TCPClient Client;

		public RemoteMatchContext(TCPClient Client)
		{
			this.Client = Client;
		}
	}
}
