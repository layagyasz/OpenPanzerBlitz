using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public abstract class Objective : Serializable
	{
		public abstract bool CanStopEarly();
		public abstract int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache);
		public abstract IEnumerable<Tile> GetTiles(Map Map);

		public int GetScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			if (Cache.ContainsKey(this)) return Cache[this];

			int score = CalculateScore(ForArmy, Match, Cache);
			Cache.Add(this, score);
			return score;
		}

		public abstract void Serialize(SerializationOutputStream Stream);
	}
}
