using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class AddPlayerAction : LobbyAction
	{
		public Player Player { get; }

		public AddPlayerAction(Player Player)
		{
			this.Player = Player;
		}

		public AddPlayerAction(SerializationInputStream Stream)
			: this(new Player(Stream)) { }

		public bool Apply(MatchLobby Lobby)
		{
			return Lobby.AddPlayer(Player);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Player);
		}
	}
}
