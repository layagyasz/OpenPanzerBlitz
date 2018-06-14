using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class LaunchAction : LobbyAction
	{
		public readonly Scenario StaticScenario;

		public Player Player
		{
			get
			{
				return null;
			}
		}

		public LaunchAction(Scenario StaticScenario)
		{
			this.StaticScenario = StaticScenario;
		}

		public LaunchAction(SerializationInputStream Stream)
			: this(new Scenario(Stream)) { }

		public bool Apply(MatchLobby Lobby)
		{
			return Lobby.Start(StaticScenario);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(StaticScenario);
		}
	}
}
