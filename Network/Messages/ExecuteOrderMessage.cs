using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ExecuteOrderMessage : GameMessage
	{
		public readonly Order Order;

		OrderSerializer _Serializer;

		public ExecuteOrderMessage(Order Order, OrderSerializer Serializer)
		{
			this.Order = Order;
			_Serializer = Serializer;
		}

		public ExecuteOrderMessage(SerializationInputStream Stream, OrderSerializer OrderSerializer)
			: this(OrderSerializer.Deserialize(Stream), OrderSerializer) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			_Serializer.Serialize(Order, Stream);
		}
	}
}
