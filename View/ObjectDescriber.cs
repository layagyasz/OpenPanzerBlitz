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

		public static string Describe(Faction Faction)
		{
			return Faction.Name;
		}

		public static string Describe(Army Army)
		{
			return Describe(Army.Configuration.Faction);
		}

		public static string Describe(ObjectiveSuccessLevel ObjectiveSuccessLevel)
		{
			return ObjectiveSuccessLevel.ToString();
		}

		public static string Describe(Unit Unit)
		{
			return string.Format("{0} (#{1})", Unit.Configuration.Name, Unit.Id);
		}

		public static string Describe(UnitConfiguration Configuration)
		{
			return Configuration.Name;
		}

		public static string Describe(TileBase TileBase)
		{
			return TileBase.ToString();
		}

		public static string Describe(TileEdge TileEdge)
		{
			return TileEdge.ToString();
		}

		public static string Describe(TilePathOverlay TilePathOverlay)
		{
			return TilePathOverlay.ToString();
		}

		public static string Describe(MapRegion Region)
		{
			return Region.Name;
		}
	}
}
