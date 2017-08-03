using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public abstract class Objective : Serializable
	{
		public readonly string UniqueKey;

		protected int _Score;

		public Objective(string UniqueKey)
		{
			this.UniqueKey = UniqueKey;
		}

		public Objective(SerializationInputStream Stream)
			: this(Stream.ReadString()) { }

		public abstract int CalculateScore(Army ForArmy, Match Match);

		public int GetScore()
		{
			return _Score;
		}

		public virtual void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueKey);
		}
	}
}
