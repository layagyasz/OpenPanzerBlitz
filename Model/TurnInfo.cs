using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public struct TurnInfo : Serializable
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

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (obj is TurnInfo)
			{
				var i = (TurnInfo)obj;
				return Army == i.Army && TurnComponent == i.TurnComponent;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Army.GetHashCode() ^ TurnComponent.GetHashCode();
		}

		public static bool operator ==(TurnInfo i1, TurnInfo i2)
		{
			if ((object)i1 == null) return (object)i2 == null;
			return i1.Equals(i2);
		}

		public static bool operator !=(TurnInfo i1, TurnInfo i2)
		{
			return !(i1 == i2);
		}

		public override string ToString()
		{
			return string.Format("[TurnInfo: Army={0}, TurnComponent={1}]", Army, TurnComponent);
		}
	}
}
