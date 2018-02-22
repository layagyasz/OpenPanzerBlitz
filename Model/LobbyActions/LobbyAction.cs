using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface LobbyAction : Serializable
	{
		Player Player { get; }
		bool Apply(MatchLobby Lobby);
	}
}
