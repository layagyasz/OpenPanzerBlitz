using System;
using System.Linq;

namespace PanzerBlitz
{
	public static class MatcherDescriber
	{
		public static string Describe(Matcher<Tile> Matcher)
		{
			if (Matcher is TileDistanceFrom) return Describe((TileDistanceFrom)Matcher);
			if (Matcher is TileElevation) return Describe((TileElevation)Matcher);
			if (Matcher is TileHasBase) return Describe((TileHasBase)Matcher);
			if (Matcher is TileHasBridge) return Describe((TileHasBridge)Matcher);
			if (Matcher is TileHasCoordinate) return Describe((TileHasCoordinate)Matcher);
			if (Matcher is TileHasEdge) return Describe((TileHasEdge)Matcher);
			if (Matcher is TileHasPath) return Describe((TileHasPath)Matcher);
			if (Matcher is TileHasUnit) return Describe((TileHasUnit)Matcher);
			if (Matcher is TileInRegion) return Describe((TileInRegion)Matcher);
			if (Matcher is TileOnEdge) return Describe((TileOnEdge)Matcher);
			if (Matcher is TileWithin) return Describe((TileWithin)Matcher);
			if (Matcher is CompositeMatcher<Tile>) return Describe((CompositeMatcher<Tile>)Matcher);
			if (Matcher is InverseMatcher<Tile>) return Describe((InverseMatcher<Tile>)Matcher);
			if (Matcher is EmptyMatcher<Tile>) return Describe((EmptyMatcher<Tile>)Matcher);

			return string.Empty;
		}

		public static string Describe(TileDistanceFrom Matcher)
		{
			return string.Format(
				"{0} {1} hexes away from tiles {2}",
				Matcher.Atleast ? "at least" : "at most",
				Matcher.Distance,
				Describe(Matcher.Matcher));
		}

		public static string Describe(TileElevation Matcher)
		{
			return string.Format(
				"at elevation of {0} {1}", Matcher.Atleast ? "at least" : "at most", Matcher.Elevation);
		}

		public static string Describe(TileHasBridge Matcher)
		{
			return "by bridge";
		}

		public static string Describe(TileHasCoordinate Matcher)
		{
			return string.Format("at {0}", ObjectDescriber.Describe(Matcher.Coordinate));
		}

		public static string Describe(TileHasBase Matcher)
		{
			return string.Format("in {0}", ObjectDescriber.Describe(Matcher.TileBase));
		}

		public static string Describe(TileHasEdge Matcher)
		{
			return string.Format("next to {0}", ObjectDescriber.Describe(Matcher.Edge));
		}

		public static string Describe(TileHasPath Matcher)
		{
			return string.Format("next to {0}", ObjectDescriber.Describe(Matcher.Path));
		}

		public static string Describe(TileHasUnit Matcher)
		{
			return string.Format("with units {0}", Describe(Matcher.Matcher));
		}

		public static string Describe(TileInRegion Matcher)
		{
			return string.Format("within {0}", ObjectDescriber.Describe(Matcher.NormalizedRegionName, '-'));
		}

		public static string Describe(TileOnEdge Matcher)
		{
			return string.Format("on {0} edge of the board", ObjectDescriber.Describe(Matcher.Edge));
		}

		public static string Describe(TileWithin Matcher)
		{
			return string.Format("within the bounds {0}", ObjectDescriber.Describe(Matcher.Zone));
		}

		public static string Describe(Matcher<Unit> Matcher)
		{
			if (Matcher is UnitHasClass) return Describe((UnitHasClass)Matcher);
			if (Matcher is UnitHasConfiguration) return Describe((UnitHasConfiguration)Matcher);
			if (Matcher is UnitHasEvacuated) return Describe((UnitHasEvacuated)Matcher);
			if (Matcher is UnitHasPosition) return Describe((UnitHasPosition)Matcher);
			if (Matcher is UnitHasReconned) return Describe((UnitHasReconned)Matcher);
			if (Matcher is UnitHasStatus) return Describe((UnitHasStatus)Matcher);
			if (Matcher is UnitIsHostile) return Describe((UnitIsHostile)Matcher);
			if (Matcher is CompositeMatcher<Unit>) return Describe((CompositeMatcher<Unit>)Matcher);
			if (Matcher is InverseMatcher<Unit>) return Describe((InverseMatcher<Unit>)Matcher);
			if (Matcher is EmptyMatcher<Unit>) return Describe((EmptyMatcher<Unit>)Matcher);

			return string.Empty;
		}

		public static string Describe(UnitHasClass Matcher)
		{
			return string.Format("of class {0}", ObjectDescriber.Describe(Matcher.UnitClass));
		}

		public static string Describe(UnitHasConfiguration Matcher)
		{
			return string.Format("of type {0}", ObjectDescriber.Describe(Matcher.UnitConfiguration));
		}

		public static string Describe(UnitHasEvacuated Matcher)
		{
			return string.Format(
				"evacuated {0} through tiles {1}",
				ObjectDescriber.Describe(Matcher.Direction),
				Describe(Matcher.Matcher));
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
			return "that are hostile";
		}

		public static string Describe(CompositeMatcher<Unit> Matcher)
		{
			return Describe(Matcher, Describe);
		}

		public static string Describe(CompositeMatcher<Tile> Matcher)
		{
			return Describe(Matcher, Describe);
		}

		public static string Describe<T>(CompositeMatcher<T> Matcher, Func<Matcher<T>, string> Describer)
		{
			return ObjectDescriber.Listify(
				Matcher.Matchers.Select(Describer),
				", ",
				Matcher.Aggregator == Aggregators.AND ? ", and " : ", or ");
		}

		public static string Describe(InverseMatcher<Unit> Matcher)
		{
			return Describe(Matcher, Describe);
		}

		public static string Describe(InverseMatcher<Tile> Matcher)
		{
			return Describe(Matcher, Describe);
		}

		public static string Describe<T>(InverseMatcher<T> Matcher, Func<Matcher<T>, string> Describer)
		{
			return "not " + Describer(Matcher.Matcher);
		}

		public static string Describe<T>(EmptyMatcher<T> Matcher)
		{
			return "anywhere";
		}
	}
}
