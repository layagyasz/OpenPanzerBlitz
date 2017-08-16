using System;

using Cardamom.Network;

namespace PanzerBlitz
{
	public class MatchLobbyContext : NetworkContext
	{
		public readonly MatchLobby Lobby;
		public readonly Chat Chat;

		public MatchLobbyContext(TCPClient Client, MatchLobby Lobby, Chat Chat)
			: base(Client)
		{
			this.Lobby = Lobby;
			this.Chat = Chat;
		}

		public MatchLobbyContext(TCPServer Server, MatchLobby Lobby, Chat Chat)
			: base(Server)
		{
			this.Lobby = Lobby;
			this.Chat = Chat;
		}

		public MatchLobbyAdapter MakeMatchLobbyAdapter()
		{
			if (IsHost) return new LocalMatchLobbyAdapter(Lobby);
			return new RemoteMatchLobbyAdapter(Client);
		}

		public ChatAdapter MakeChatAdapter()
		{
			if (IsHost) return new LocalChatAdapter(Chat);
			return new RemoteChatAdapter(Client);
		}
	}
}
