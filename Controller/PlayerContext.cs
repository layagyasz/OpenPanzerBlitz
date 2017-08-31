using System;

using Cardamom.Network;

namespace PanzerBlitz
{
	public class PlayerContext : NetworkContext
	{
		public readonly Player Player;

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
	}
}
