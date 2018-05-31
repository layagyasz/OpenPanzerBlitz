using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class Player : Serializable
	{
		public readonly OnlineId Id;
		public readonly string Name;

		public Player(OnlineId Id, string Name)
		{
			this.Id = Id;
			this.Name = Name;
		}

		public Player(SerializationInputStream Stream)
			: this(new OnlineId(Stream), Stream.ReadString()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Id);
			Stream.Write(Name);
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (obj is Player)
			{
				var o = (Player)obj;
				return Id == o.Id && Name == o.Name;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Name.GetHashCode();
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

		public override string ToString()
		{
			return string.Format("[Player: Id={0}, Name={1}]", Id, Name);
		}
	}
}
