using System;

using Cardamom.Network;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class GameMessageSerializer : MessageAdapter
	{
		static readonly Type[] _Messages =
		{
			typeof(ExecuteOrderRequest)
		};

		Func<SerializationInputStream, Message>[] _Deserializers;

		public override Type[] Messages
		{
			get
			{
				return _Messages;
			}
		}
		public override Func<SerializationInputStream, Message>[] Deserializers
		{
			get
			{
				return _Deserializers;
			}
		}

		OrderSerializer _OrderSerializer;

		public GameMessageSerializer(OrderSerializer OrderSerializer)
		{
			_OrderSerializer = OrderSerializer;
			_Deserializers = new Func<SerializationInputStream, Message>[]
			{
				i => new ExecuteOrderRequest(i, _OrderSerializer)
			};
		}
	}
}
