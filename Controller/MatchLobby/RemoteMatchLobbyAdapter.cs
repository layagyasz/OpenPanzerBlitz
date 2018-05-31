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
			return _Client.Call(
				new ApplyLobbyActionRequest(new SetPlayerArmyAction(Player, Army))).Get<BooleanResponse>().Value;
		}

		public bool SetPlayerReady(Player Player, bool Ready)
		{
			return _Client.Call(
				new ApplyLobbyActionRequest(new SetPlayerReadyAction(Player, Ready))).Get<BooleanResponse>().Value;
		}

		public bool Start()
		{
			return false;
		}
	}
}
