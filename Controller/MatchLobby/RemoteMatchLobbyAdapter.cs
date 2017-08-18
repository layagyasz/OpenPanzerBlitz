using System;

using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class RemoteMatchLobbyAdapter : MatchLobbyAdapter
	{
		TCPClient _Client;

		public RemoteMatchLobbyAdapter(TCPClient Client)
		{
			_Client = Client;
		}

		public bool SetScenario(Scenario Scenario)
		{
			return false;
		}

		public bool SetPlayerArmy(Player Player, ArmyConfiguration Army)
		{
			return ((BooleanResponse)_Client.Call(
				new ApplyLobbyActionRequest(new SetPlayerArmyAction(Player, Army))).Get()).Value;
		}

		public bool SetPlayerReady(Player Player, bool Ready)
		{
			return ((BooleanResponse)_Client.Call(
				new ApplyLobbyActionRequest(new SetPlayerReadyAction(Player, Ready))).Get()).Value;
		}

		public bool Start()
		{
			return false;
		}
	}
}
