using System;
using System.Linq;

namespace PanzerBlitz
{
	public class MatcherDescriber
	{
		public static string Describe(Matcher<Tile> Matcher)
		{
			if (Matcher is TileDistanceFrom) return Describe((TileDistanceFrom)Matcher);
			if (Matcher is TileElevation) return Describe((TileElevation)Matcher);
			if (Matcher is TileHasCoordinate) return Describe((TileHasCoordinate)Matcher);
			if (Matcher is TileHasEdge) return Describe((TileHasEdge)Matcher);
			if (Matcher is TileHasUnit) return Describe((TileHasUnit)Matcher);
			if (Matcher is TileInRegion) return Describe((TileInRegion)Matcher);
			if (Matcher is TileOnEdge) return Describe((TileOnEdge)Matcher);
			if (Matcher is TileWithin) return Describe((TileWithin)Matcher);
			if (Matcher is CompositeMatcher<Tile>) return Describe((CompositeMatcher<Tile>)Matcher);
			if (Matcher is InverseMatcher<Tile>) return Describe((InverseMatcher<Tile>)Matcher);
			if (Matcher is EmptyMatcher<Tile>) return Describe((EmptyMatcher<Tile>)Matcher);

			return Describe(Matcher);
		}

		public static string Describe(TileDistanceFrom Matcher)
		{
			return string.Format(
				"{0} {1} hexes away from tiles that {2}",
				Matcher.Atleast ? "at least" : "at most",
				Matcher.Distance,
				Describe(Matcher.Matcher));
		}

		public static string Describe(TileElevation Matcher)
		{
			return string.Format(
				"at elevation of {0} {1}", Matcher.Atleast ? "at least" : "at most", Matcher.Elevation);
		}

		public static string Describe(TileHasCoordinate Matcher)
		{
			return string.Format("at {0}", ObjectDescriber.Describe(Matcher.Coordinate));
		}

		public static string Describe(TileHasEdge Matcher)
		{
			return string.Format("next to {0}", ObjectDescriber.Describe(Matcher.Edge));
		}

		public static string Describe(TileHasUnit Matcher)
		{
			return string.Format("contain units {0}", Describe(Matcher.Matcher));
		}

		public static string Describe(TileInRegion Matcher)
		{
			return string.Format("within {0}", Matcher.NormalizedRegionName);
		}

		public static string Describe(TileOnEdge Matcher)
		{
			return string.Format("on the {0} edge of the board", ObjectDescriber.Describe(Matcher.Edge));
		}

		public static string Describe(TileWithin Matcher)
		{
			return string.Format("within the bounds {0}", ObjectDescriber.Describe(Matcher.Zone));
		}

		public static string Describe(Matcher<Unit> Matcher)
		{
			if (Matcher is UnitHasConfiguration) return Describe((UnitHasConfiguration)Matcher);
			if (Matcher is UnitHasEvacuated) return Describe((UnitHasEvacuated)Matcher);
			if (Matcher is UnitHasPosition) return Describe((UnitHasPosition)Matcher);
			if (Matcher is UnitHasReconned) return Describe((UnitHasReconned)Matcher);
			if (Matcher is UnitHasStatus) return Describe((UnitHasStatus)Matcher);
			if (Matcher is UnitIsHostile) return Describe((UnitIsHostile)Matcher);
			if (Matcher is CompositeMatcher<Unit>) return Describe((CompositeMatcher<Unit>)Matcher);
			if (Matcher is InverseMatcher<Unit>) return Describe((InverseMatcher<Unit>)Matcher);
			if (Matcher is EmptyMatcher<Unit>) return Describe((EmptyMatcher<Unit>)Matcher);

			return Describe<Unit>(Matcher);
		}

		public static string Describe(UnitHasConfiguration Matcher)
		{
			return string.Format("of type {0}", ObjectDescriber.Describe(Matcher.UnitConfiguration));
		}

		public static string Describe(UnitHasEvacuated Matcher)
		{
			return string.Format("evacuated {0}", ObjectDescriber.Describe(Matcher.Direction));
		}

		public static string Describe(UnitHasPosition Matcher)
		{
			return Describe(Matcher.Matcher);
		}

		public static string Describe(UnitHasReconned Matcher)
		{
			return string.Format("reconned {0}", ObjectDescriber.Describe(Matcher.Direction));
		}

		public static string Describe(UnitHasStatus Matcher)
		{
			return ObjectDescriber.Describe(Matcher.Status);
		}

		public static string Describe(UnitIsHostile Matcher)
		{
			return "are hostile";
		}

		public static string Describe<T>(Matcher<T> Matcher)
		{
			if (Matcher is Matcher<Tile>) return Describe((Matcher<Tile>)Matcher);
			if (Matcher is Matcher<Unit>) return Describe((Matcher<Unit>)Matcher);

			return "";
		}

		public static string Describe<T>(CompositeMatcher<T> Matcher)
		{
			return ObjectDescriber.Listify(
				Matcher.Matchers.Select(Describe),
				", ",
				Matcher.Aggregator == CompositeMatcher<T>.AND ? ", and " : ", or ");
		}

		public static string Describe<T>(InverseMatcher<T> Matcher)
		{
			return "not " + Describe(Matcher.Matcher);
		}

		public static string Describe<T>(EmptyMatcher<T> Matcher)
		{
			return "anywhere";
		}
	}
}
