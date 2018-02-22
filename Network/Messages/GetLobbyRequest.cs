using Cardamom.Network;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class GetLobbyRequest : RPCRequest
	{
		public GetLobbyRequest() { }

		public GetLobbyRequest(SerializationInputStream Stream)
			: base(Stream) { }
	}
}
