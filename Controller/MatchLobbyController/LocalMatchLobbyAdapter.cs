using System;
namespace PanzerBlitz
{
	public class LocalMatchLobbyAdapter : MatchLobbyAdapter
	{
		MatchLobby _Lobby;

		public LocalMatchLobbyAdapter(MatchLobby Lobby)
		{
			_Lobby = Lobby;
		}

		public bool ApplyAction(LobbyAction Action)
		{
			return _Lobby.ApplyAction(Action);
		}
	}
}
