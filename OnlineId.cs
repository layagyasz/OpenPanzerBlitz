using Cardamom.Serialization;

namespace PanzerBlitz
{
	public struct OnlineId : Serializable
	{
		public static OnlineId Permanent(long Id)
		{
			return new OnlineId(Id, false);
		}

		public static OnlineId Temporary(long Id)
		{
			return new OnlineId(Id, true);
		}

		public long Id { get; }
		public bool IsTemporary { get; }

		OnlineId(long Id, bool IsTemporary)
		{
			this.Id = Id;
			this.IsTemporary = IsTemporary;
		}

		public OnlineId(SerializationInputStream Stream)
		{
			Id = Stream.ReadInt64();
			IsTemporary = Stream.ReadBoolean();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Id);
			Stream.Write(IsTemporary);
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (obj is OnlineId)
			{
				var o = (OnlineId)obj;
				return Id == o.Id && IsTemporary == o.IsTemporary;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ IsTemporary.GetHashCode();
		}

		public static bool operator ==(OnlineId i1, OnlineId i2)
		{
			return i1.Equals(i2);
		}

		public static bool operator !=(OnlineId i1, OnlineId i2)
		{
			return !(i1 == i2);
		}

		public override string ToString()
		{
			return string.Format("[OnlineId: Id={0}, IsTemporary={1}]", Id, IsTemporary);
		}
	}
}
