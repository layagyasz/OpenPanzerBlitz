using Cardamom.Network;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ExecuteOrderRequest : RPCRequest
	{
		public readonly Order Order;

		readonly OrderSerializer _Serializer;

		public ExecuteOrderRequest(Order Order, OrderSerializer Serializer)
		{
			this.Order = Order;
			_Serializer = Serializer;
		}

		public ExecuteOrderRequest(SerializationInputStream Stream, OrderSerializer OrderSerializer)
			: base(Stream)
		{
			Order = OrderSerializer.Deserialize(Stream);
			_Serializer = OrderSerializer;
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			_Serializer.Serialize(Order, Stream);
		}
	}
}
