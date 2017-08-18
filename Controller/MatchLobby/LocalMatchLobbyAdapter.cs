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

		public bool SetScenario(Scenario Scenario)
		{
			return _Lobby.ApplyAction(new SetScenarioAction(Scenario));
		}

		public bool SetPlayerArmy(Player Player, ArmyConfiguration Army)
		{
			return _Lobby.ApplyAction(new SetPlayerArmyAction(Player, Army));
		}

		public bool SetPlayerReady(Player Player, bool Ready)
		{
			return _Lobby.ApplyAction(new SetPlayerReadyAction(Player, Ready));
		}

		public bool Start()
		{
			return _Lobby.ApplyAction(new LaunchAction());
		}
	}
}
