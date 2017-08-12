using System;

using Cardamom.Network;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class GameMessageSerializer : SerializableAdapter
	{
		OrderSerializer _OrderSerializer;

		public GameMessageSerializer(OrderSerializer OrderSerializer)
			: base(new Tuple<Type, Func<SerializationInputStream, Serializable>>[]
		{
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(ExecuteOrderRequest), i => new ExecuteOrderRequest(i, OrderSerializer))
		})
		{
			_OrderSerializer = OrderSerializer;
		}
	}
}
