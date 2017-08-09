using System;

using Cardamom.Network;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ValidateOrderRequest : RPCRequest
	{
		public readonly Order Order;

		OrderSerializer _Serializer;

		public ValidateOrderRequest(Order Order, OrderSerializer Serializer)
		{
			this.Order = Order;
			_Serializer = Serializer;
		}

		public ValidateOrderRequest(SerializationInputStream Stream, OrderSerializer OrderSerializer)
			: base(Stream)
		{
			Order = OrderSerializer.Deserialize(Stream);
			_Serializer = OrderSerializer;
		}

		public override void Serialize(SerializationOutputStream Stream)
		{
			_Serializer.Serialize(Order, Stream);
		}
	}
}
