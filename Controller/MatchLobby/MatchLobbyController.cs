using System;

using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class MatchLobbyController
	{
		MatchLobbyAdapter _LobbyAdapter;
		MatchLobbyScreen _LobbyScreen;

		public MatchLobbyController(MatchLobbyAdapter LobbyAdapter, MatchLobbyScreen LobbyScreen)
		{
			_LobbyAdapter = LobbyAdapter;
			_LobbyScreen = LobbyScreen;
			_LobbyScreen.OnArmyConfigurationSelected += HandlePlayerArmyChanged;
			_LobbyScreen.OnPlayerReadyStateChanged += HandlePlayerReadyStateChanged;
		}

		void HandlePlayerArmyChanged(object Sender, ValuedEventArgs<Tuple<Player, ArmyConfiguration>> E)
		{
			_LobbyAdapter.SetArmyPlayer(E.Value.Item1, E.Value.Item2);
		}

		void HandlePlayerReadyStateChanged(object Sender, ValuedEventArgs<Tuple<Player, bool>> E)
		{
			_LobbyAdapter.SetPlayerReady(E.Value.Item1, E.Value.Item2);
		}
	}
}
