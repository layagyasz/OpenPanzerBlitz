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
			typeof(ClearMinefieldOrder),
			typeof(CloseAssaultAttackOrder),
			typeof(ConvoyOrderDeployOrder),
			typeof(DismountOrder),
			typeof(EntryTileDeployOrder),
			typeof(EvacuateOrder),
			typeof(LoadOrder),
			typeof(MinefieldAttackOrder),
			typeof(MountOrder),
			typeof(MovementDeployOrder),
			typeof(MovementOrder),
			typeof(NextPhaseOrder),
			typeof(NormalAttackOrder),
			typeof(OverrunAttackOrder),
			typeof(PositionalDeployOrder),
			typeof(ReconOrder),
			typeof(ResetOrder),
			typeof(UnloadOrder)
		};

		static readonly Func<SerializationInputStream, List<GameObject>, Order>[] DESERIALIZERS =
		{
			(i, j) => new ClearMinefieldOrder(i, j),
			(i, j) => new CloseAssaultAttackOrder(i, j),
			(i, j) => new ConvoyOrderDeployOrder(i, j),
			(i, j) => new DismountOrder(i, j),
			(i, j) => new EntryTileDeployOrder(i, j),
			(i, j) => new EvacuateOrder(i, j),
			(i, j) => new LoadOrder(i, j),
			(i, j) => new MinefieldAttackOrder(i, j),
			(i, j) => new MountOrder(i, j),
			(i, j) => new MovementDeployOrder(i, j),
			(i, j) => new MovementOrder(i, j),
			(i, j) => new NextPhaseOrder(i, j),
			(i, j) => new NormalAttackOrder(i, j),
			(i, j) => new OverrunAttackOrder(i, j),
			(i, j) => new PositionalDeployOrder(i, j),
			(i, j) => new ReconOrder(i, j),
			(i, j) => new ResetOrder(i, j),
			(i, j) => new UnloadOrder(i, j)
		};

		readonly List<GameObject> _GameObjects = new List<GameObject>();

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
			return DESERIALIZERS[Stream.ReadByte()](Stream, _GameObjects);
		}
	}
}
