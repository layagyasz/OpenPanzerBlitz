using System;

using Cardamom.Network.Responses;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class MatchMessageSerializer : SerializableAdapter
	{
		readonly OrderSerializer _OrderSerializer;

		public MatchMessageSerializer(OrderSerializer OrderSerializer)
			: base(new Tuple<Type, Func<SerializationInputStream, Serializable>>[]
		{
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(ExecuteOrderRequest), i => new ExecuteOrderRequest(i, OrderSerializer)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(ValidateOrderRequest), i => new ValidateOrderRequest(i, OrderSerializer)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(BooleanResponse), i => new BooleanResponse(i))
		})
		{
			_OrderSerializer = OrderSerializer;
		}
	}
}
