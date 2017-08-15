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

		public bool SetArmyPlayer(Player Player, ArmyConfiguration Army)
		{
			return false;
		}

		public bool SetPlayerReady(Player Player, bool Ready)
		{
			return false;
		}
	}
}
