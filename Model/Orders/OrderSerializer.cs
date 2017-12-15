using System;
using System.Collections.Generic;
using System.Linq;

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
			typeof(ReconOrder),
			typeof(EvacuateOrder),
			typeof(MountOrder),
			typeof(DismountOrder),
			typeof(ResetOrder),
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
			(i, j) => new ReconOrder(i, j),
			(i, j) => new EvacuateOrder(i, j),
			(i, j) => new MountOrder(i, j),
			(i, j) => new DismountOrder(i, j),
			(i, j) => new ResetOrder(i, j),
			(i, j) => new NextPhaseOrder(i, j),
		};

		List<GameObject> _GameObjects = new List<GameObject>();

		public OrderSerializer(Match Match)
		{
			_GameObjects = Match.GetGameObjects().ToList();
			_GameObjects.Sort((i, j) => i.Id.CompareTo(j.Id));
			_GameObjects.Insert(0, null);

			foreach (Army a in Match.Armies) a.OnUnitAdded += (sender, e) => _GameObjects.Add(e.Unit);
		}

		public void Serialize(Order Order, SerializationOutputStream Stream)
		{
			Stream.Write((byte)Array.IndexOf(ORDER_TYPES, Order.GetType()));
			Stream.Write(Order);
		}

		public Order Deserialize(SerializationInputStream Stream)
		{
			Order o = DESERIALIZERS[Stream.ReadByte()](Stream, _GameObjects);
			Console.WriteLine("[DEBUG SERIALIZER] {0}", o);
			return o;
		}
	}
}
