using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ResetOrder : Order
	{
		public Army Army { get; }
		public readonly bool CompleteReset;

		public ResetOrder(Army Army, bool CompleteReset)
		{
			this.Army = Army;
			this.CompleteReset = CompleteReset;
		}

		public ResetOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Army)Objects[Stream.ReadInt32()], Stream.ReadBoolean()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Army.Id);
			Stream.Write(CompleteReset);
		}

		public OrderStatus Execute(Random Random)
		{
			Army.Reset(CompleteReset);
			return OrderStatus.FINISHED;
		}
	}
}
