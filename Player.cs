﻿using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class Player : Serializable
	{
		public readonly int TemporaryId;
		public readonly int PermanentId;
		public readonly string Name;

		public Player(int Id, string Name, bool IsTemporaryId)
		{
			this.Name = Name;
			if (IsTemporaryId) TemporaryId = Id;
			else PermanentId = Id;
		}

		public Player(SerializationInputStream Stream)
			: this(Stream.ReadInt32(), Stream.ReadString(), Stream.ReadBoolean()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(TemporaryId == 0 ? PermanentId : TemporaryId);
			Stream.Write(Name);
			Stream.Write(PermanentId == 0);
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (obj is Player)
			{
				var o = (Player)obj;
				return TemporaryId == o.TemporaryId && PermanentId == o.PermanentId && Name == o.Name;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return TemporaryId.GetHashCode() ^ PermanentId.GetHashCode() ^ Name.GetHashCode();
		}

		public static bool operator ==(Player p1, Player p2)
		{
			if ((object)p1 == null) return (object)p2 == null;
			return p1.Equals(p2);
		}

		public static bool operator !=(Player p1, Player p2)
		{
			return !(p1 == p2);
		}
	}
}
