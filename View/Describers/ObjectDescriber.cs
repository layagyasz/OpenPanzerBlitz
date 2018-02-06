using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Planar;

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

		public static string Describe(Environment Environment)
		{
			return Environment.UniqueKey;
		}

		public static string Describe(Coordinate Coordinate)
		{
			return string.Format("({0}, {1})", Coordinate.X, Coordinate.Y);
		}

		public static string Describe(Polygon Zone)
		{
			return string.Format(
				"[{0}]",
				string.Join(", ", Zone.Segments.Select(i => Describe(new Coordinate((int)i.Point.X, (int)i.Point.Y)))));
		}

		public static string Describe(ObjectiveSuccessTrigger Trigger)
		{
			return string.Format(
				"{0} - {1}",
				Describe(Trigger.SuccessLevel),
				Sentencify(
					ObjectiveDescriber.Describe(
						new TriggerObjective(Trigger.Objective, Trigger.Threshold, Trigger.Invert))));
		}

		public static string Sentencify(string Input)
		{
			if (Input.Length == 0) return Input;
			return char.ToUpper(Input[0]) + Input.Substring(1) + ".";
		}

		public static string Listify(IEnumerable<string> Parts, string Joiner, string TerminalJoiner)
		{
			return string.Format(
				"{0}{1}{2}", string.Join(Joiner, Parts.Take(Parts.Count() - 1)), TerminalJoiner, Parts.Last());
		}
	}
}
