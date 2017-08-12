using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class Player : Serializable
	{
		public readonly string Name;

		public Player(string Name)
		{
			this.Name = Name;
		}

		public Player(SerializationInputStream Stream)
			: this(Stream.ReadString()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Name);
		}

		public override bool Equals(object obj)
		{
			return obj is Player && ((Player)obj).Name == Name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public static bool operator ==(Player p1, Player p2)
		{
			return p1.Equals(p2);
		}

		public static bool operator !=(Player p1, Player p2)
		{
			return !(p1 == p2);
		}
	}
}
