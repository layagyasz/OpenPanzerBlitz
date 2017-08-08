using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TurnInfo : Serializable
	{
		public readonly Army Army;
		public readonly TurnComponent TurnComponent;

		public TurnInfo(Army Army, TurnComponent TurnComponent)
		{
			this.Army = Army;
			this.TurnComponent = TurnComponent;
		}

		public TurnInfo(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Army)Objects[Stream.ReadInt32()], (TurnComponent)Stream.ReadByte()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Army.Id);
			Stream.Write((byte)TurnComponent);
		}
	}
}
