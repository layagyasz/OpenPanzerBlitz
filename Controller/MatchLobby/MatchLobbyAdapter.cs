using System;
namespace PanzerBlitz
{
	public interface MatchLobbyAdapter
	{
		bool SetArmyPlayer(Player Player, ArmyConfiguration Army);
		bool SetPlayerReady(Player Player, bool Ready);
	}
}
