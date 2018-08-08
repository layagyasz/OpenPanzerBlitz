using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public abstract class Objective : Serializable
	{
		public abstract bool CanStopEarly();
		public abstract int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache);
		public abstract int? GetMaximumScore(Objective Objective, Army ForArmy, Match Match);
		public abstract IEnumerable<Tile> GetTiles(Map Map);

		public int GetScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			if (Cache.ContainsKey(this)) return Cache[this];

			var score = CalculateScore(ForArmy, Match, Cache);
			Cache.Add(this, score);
			return score;
		}

		public abstract void Serialize(SerializationOutputStream Stream);
	}
}
