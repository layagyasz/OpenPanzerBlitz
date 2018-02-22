using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class SetScenarioAction : LobbyAction
	{
		public readonly Scenario Scenario;

		public Player Player
		{
			get
			{
				return null;
			}
		}

		public SetScenarioAction(Scenario Scenario)
		{
			this.Scenario = Scenario;
		}

		public SetScenarioAction(SerializationInputStream Stream)
			: this(new Scenario(Stream)) { }

		public bool Apply(MatchLobby Lobby)
		{
			return Lobby.SetScenario(Scenario);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Scenario);
		}
	}
}
