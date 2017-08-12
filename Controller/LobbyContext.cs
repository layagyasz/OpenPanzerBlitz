using System;

using Cardamom.Network;

namespace PanzerBlitz
{
	public class LobbyContext : NetworkContext
	{
		public readonly MatchLobby Lobby;

		public LobbyContext(TCPClient Client, MatchLobby Lobby)
			: base(Client)
		{
			this.Lobby = Lobby;
		}

		public LobbyContext(TCPServer Server, MatchLobby Lobby)
			: base(Server)
		{
			this.Lobby = Lobby;
		}
	}
}
