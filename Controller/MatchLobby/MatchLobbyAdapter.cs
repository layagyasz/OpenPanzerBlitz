using System;
namespace PanzerBlitz
{
	public interface MatchLobbyAdapter
	{
		bool SetScenario(Scenario Scenario);
		bool SetPlayerArmy(Player Player, ArmyConfiguration Army);
		bool SetPlayerReady(Player Player, bool Ready);
		bool Start();
	}
}
