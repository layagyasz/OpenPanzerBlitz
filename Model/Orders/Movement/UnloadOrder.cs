using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnloadOrder : Order
	{
		public readonly Unit Carrier;
		public readonly bool UseMovement;

		public Army Army
		{
			get
			{
				return Carrier.Army;
			}
		}

		public UnloadOrder(Unit Carrier, bool UseMovement = true)
		{
			this.Carrier = Carrier;
			this.UseMovement = UseMovement;
		}

		public UnloadOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Unit)Objects[Stream.ReadInt32()], Stream.ReadBoolean()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Carrier.Id);
			Stream.Write(UseMovement);
		}

		public NoUnloadReason Validate()
		{
			return Carrier.CanUnload();
		}

		public OrderStatus Execute(Random Random)
		{
			if (Validate() == NoUnloadReason.NONE)
			{
				Carrier.Unload(UseMovement);
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}
	}
}
