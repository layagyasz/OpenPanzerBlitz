using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class LaunchAction : LobbyAction
	{
		public Player Player
		{
			get
			{
				return null;
			}
		}

		public LaunchAction() { }

		public LaunchAction(SerializationInputStream Stream) { }

		public bool Apply(MatchLobby Lobby)
		{
			return Lobby.Start();
		}

		public void Serialize(SerializationOutputStream Stream) { }
	}
}
