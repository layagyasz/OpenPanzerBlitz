using Cardamom.Network;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class GetUnitConfigurationPacksRequest : RPCRequest
	{
		public GetUnitConfigurationPacksRequest() { }

		public GetUnitConfigurationPacksRequest(SerializationInputStream Stream)
			: base(Stream) { }
	}
}
