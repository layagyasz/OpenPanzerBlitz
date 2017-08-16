using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class SetPlayerReadyAction : LobbyAction
	{
		public readonly Player Player;
		public readonly bool Ready;

		public SetPlayerReadyAction(Player Player, bool Ready)
		{
			this.Player = Player;
			this.Ready = Ready;
		}

		public SetPlayerReadyAction(SerializationInputStream Stream)
			: this(new Player(Stream), Stream.ReadBoolean()) { }

		public bool Apply(MatchLobby Lobby)
		{
			return Lobby.SetPlayerReady(Player, Ready);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Player);
			Stream.Write(Ready);
		}
	}
}
