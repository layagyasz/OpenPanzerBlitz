using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public static class SingleAttackOrderSerializer
	{
		static readonly Type[] SINGLE_ATTACK_ORDER_TYPES =
		{
			typeof(NormalSingleAttackOrder),
			typeof(OverrunSingleAttackOrder),
			typeof(MinefieldSingleAttackOrder)
		};

		static readonly Func<SerializationInputStream, List<GameObject>, SingleAttackOrder>[] DESERIALIZERS =
		{
			(i, j) => new NormalSingleAttackOrder(i, j),
			(i, j) => new OverrunSingleAttackOrder(i, j),
			(i, j) => new MinefieldSingleAttackOrder(i, j)
		};

		public static void Serialize(SingleAttackOrder Order, SerializationOutputStream Stream)
		{
			Stream.Write((byte)Array.IndexOf(SINGLE_ATTACK_ORDER_TYPES, Order.GetType()));
			Stream.Write(Order);
		}

		public static SingleAttackOrder Deserialize(SerializationInputStream Stream, List<GameObject> Objects)
		{
			return DESERIALIZERS[Stream.ReadByte()](Stream, Objects);
		}
	}
}
