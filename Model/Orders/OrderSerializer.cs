using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class OrderSerializer
	{
		static readonly Type[] ORDER_TYPES =
		{
			typeof(AttackOrder),
			typeof(ConvoyOrderDeployOrder),
			typeof(EntryTileDeployOrder),
			typeof(MovementDeployOrder),
			typeof(PositionalDeployOrder),
			typeof(LoadOrder),
			typeof(MovementOrder),
			typeof(UnloadOrder),
			typeof(NextPhaseOrder)
		};

		static readonly Func<SerializationInputStream, List<GameObject>, Order>[] DESERIALIZERS =
		{
			(i, j) => new AttackOrder(i, j),
			(i, j) => new ConvoyOrderDeployOrder(i, j),
			(i, j) => new EntryTileDeployOrder(i, j),
			(i, j) => new MovementDeployOrder(i, j),
			(i, j) => new PositionalDeployOrder(i, j),
			(i, j) => new LoadOrder(i, j),
			(i, j) => new MovementOrder(i, j),
			(i, j) => new UnloadOrder(i, j),
			(i, j) => new NextPhaseOrder(i, j),
		};

		List<GameObject> GameObjects = new List<GameObject>();

		public void SerializeOrder(Order Order, SerializationOutputStream Stream)
		{
			Stream.Write((byte)Array.IndexOf(ORDER_TYPES, Order.GetType()));
			Stream.Write(Order);
		}

		public Order DeserializeOrder(SerializationInputStream Stream)
		{
			return DESERIALIZERS[Stream.ReadByte()](Stream, GameObjects);
		}
	}
}
