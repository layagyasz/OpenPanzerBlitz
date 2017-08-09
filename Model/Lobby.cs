using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class Lobby
	{
		public EventHandler OnLobbyUpdated;

		public readonly Scenario Scenario;

		Dictionary<ArmyConfiguration, Player> _ArmyPlayers;
		List<Player> _Players = new List<Player>();

		public Lobby(Scenario Scenario)
		{
			this.Scenario = Scenario;
			_ArmyPlayers = Scenario.ArmyConfigurations.ToDictionary(i => i, i => (Player)null);
		}

		public void AddPlayer(Player Player)
		{
			_Players.Add(Player);
			if (OnLobbyUpdated != null) OnLobbyUpdated(this, EventArgs.Empty);
		}

		public void SetArmyPlayer(ArmyConfiguration Army, Player Player)
		{
			if (_Players.Contains(Player))
			{
				_ArmyPlayers[Army] = Player;
				if (OnLobbyUpdated != null) OnLobbyUpdated(this, EventArgs.Empty);
			}
		}
	}
}
