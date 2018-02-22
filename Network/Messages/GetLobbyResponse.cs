using Cardamom.Network;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class GetLobbyResponse : RPCResponse
	{
		public readonly MatchLobby Lobby;

		public GetLobbyResponse(MatchLobby Lobby)
		{
			this.Lobby = Lobby;
		}

		public GetLobbyResponse(SerializationInputStream Stream)
			: base(Stream)
		{
			Lobby = new MatchLobby(Stream);
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			Stream.Write(Lobby);
		}
	}
}
