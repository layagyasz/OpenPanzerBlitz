using System;

namespace PanzerBlitz
{
	public enum Rarity
	{
		NONE,

		COMMON,
		LIMITED,
		VERY_LIMITED,
		SCARCE,
		VERY_SCARCE,
	}

	public static class RarityExtensions
	{
		public static Rarity GetRarity(double Value)
		{
			if (Value < 8.0) return Rarity.COMMON;
			if (Value < 12.0) return Rarity.LIMITED;
			if (Value < 16.0) return Rarity.VERY_LIMITED;
			if (Value < 20.0) return Rarity.SCARCE;
			return Rarity.VERY_SCARCE;
		}

		public static ConsoleColor GetConsoleColor(this Rarity Rarity)
		{
			switch (Rarity)
			{
				case Rarity.COMMON: return ConsoleColor.White;
				case Rarity.LIMITED: return ConsoleColor.Green;
				case Rarity.VERY_LIMITED: return ConsoleColor.Cyan;
				case Rarity.SCARCE: return ConsoleColor.Magenta;
				case Rarity.VERY_SCARCE: return ConsoleColor.Yellow;
				default: return ConsoleColor.Gray;
			}
		}
	}
}
