using System;

using Cardamom.Network;

namespace PanzerBlitz
{
	public class MatchLobbyContext : NetworkContext
	{
		public readonly MatchLobby Lobby;

		public MatchLobbyContext(TCPClient Client, MatchLobby Lobby)
			: base(Client)
		{
			this.Lobby = Lobby;
		}

		public MatchLobbyContext(TCPServer Server, MatchLobby Lobby)
			: base(Server)
		{
			this.Lobby = Lobby;
		}
	}
}
