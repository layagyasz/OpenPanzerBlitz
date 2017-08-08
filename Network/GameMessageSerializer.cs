using System;

using Cardamom.Network;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class GameMessageSerializer : RPCAdapter
	{
		static readonly Type[] _Messages =
		{
			typeof(ExecuteOrderMessage)
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
			_Deserializers = new Func<SerializationInputStream, GameMessage>[]
			{
				i => new ExecuteOrderMessage(i, _OrderSerializer)
			};
		}
	}
}
