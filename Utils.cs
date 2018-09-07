using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public static class Utils
	{
		public static bool[] ToArray<T>(IEnumerable<T> Values)
		{
			var values = new bool[Enum.GetNames(typeof(T)).Length];
			foreach (var v in Values) values[(int)(object)v] = true;
			return values;
		}

		public static IEnumerable<T> ToValues<T>(bool[] Array)
		{
			for (int i = 0; i < Array.Length; ++i)
			{
				if (Array[i]) yield return (T)(object)i;
			}
		}

		public static T Choice<T>(Random Random, IEnumerable<T> Values)
		{
			return Values.ElementAtOrDefault(Random.Next(0, Values.Count()));
		}
	}
}
