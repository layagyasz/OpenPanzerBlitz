using System;

namespace PanzerBlitz
{
	public static class Aggregators
	{
		public static readonly Func<bool, bool, bool> AND = (i, j) => i && j;
		public static readonly Func<bool, bool, bool> OR = (i, j) => i || j;

		public static readonly Func<bool, bool, bool>[] AGGREGATORS = { AND, OR };
	}
}
