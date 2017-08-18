using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class NextPhaseOrder : Order
	{
		public Army Army { get; }

		public NextPhaseOrder(Army Army)
		{
			this.Army = Army;
		}

		public NextPhaseOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Army)Objects[Stream.ReadInt32()]) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Army.Id);
		}

		public bool Validate()
		{
			return true;
		}

		public bool Execute(Random Random)
		{
			return true;
		}
	}
}
