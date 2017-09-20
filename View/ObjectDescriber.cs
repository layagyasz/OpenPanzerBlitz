using System;
namespace PanzerBlitz
{
	public static class ObjectDescriber
	{
		public static string Describe(object Object)
		{
			if (Object is Unit) return Describe((Unit)Object);
			return Object.ToString();
		}

		public static string Describe(Unit Unit)
		{
			return Unit.Configuration.Name;
		}
	}
}
