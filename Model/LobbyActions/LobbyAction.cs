using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface LobbyAction : Serializable
	{
		bool Apply(MatchLobby Lobby);
	}
}
