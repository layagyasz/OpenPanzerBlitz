﻿using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class SetPlayerArmyAction : LobbyAction
	{
		public Player Player { get; }
		public readonly string ArmyConfigurationKey;

		public SetPlayerArmyAction(Player Player, ArmyConfiguration ArmyConfiguration)
		{
			this.Player = Player;
			ArmyConfigurationKey = ArmyConfiguration == null ? "" : ArmyConfiguration.UniqueKey;
		}

		public SetPlayerArmyAction(SerializationInputStream Stream)
		{
			Player = new Player(Stream);
			ArmyConfigurationKey = Stream.ReadString();
		}

		public bool Apply(MatchLobby Lobby)
		{
			return Lobby.SetPlayerArmy(
				Player, Lobby.Scenario.ArmyConfigurations.FirstOrDefault(i => i.UniqueKey == ArmyConfigurationKey));
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Player);
			Stream.Write(ArmyConfigurationKey);
		}
	}
}
