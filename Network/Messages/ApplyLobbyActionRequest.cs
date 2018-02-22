using Cardamom.Network;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ApplyLobbyActionRequest : RPCRequest
	{
		public readonly LobbyAction Action;

		public ApplyLobbyActionRequest(LobbyAction Action)
		{
			this.Action = Action;
		}

		public ApplyLobbyActionRequest(SerializationInputStream Stream)
			: base(Stream)
		{
			Action = (LobbyAction)LobbyActionSerializer.Instance.Deserialize(Stream);
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			LobbyActionSerializer.Instance.Serialize(Action, Stream);
		}
	}
}
