using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class MatcherSerializer : SerializableAdapter
	{
		public static readonly MatcherSerializer Instance = new MatcherSerializer();

		MatcherSerializer()
			: base(new Type[]
		{
			typeof(CompositeMatcher<Tile>),
			typeof(EmptyMatcher<Tile>),
			typeof(InverseMatcher<Tile>),

			typeof(TileDistanceFrom),
			typeof(TileElevation),
			typeof(TileHasBridge),
			typeof(TileHasCoordinate),
			typeof(TileHasEdge),
			typeof(TileHasUnit),
			typeof(TileInRegion),
			typeof(TileOnEdge),
			typeof(TileWithin),

			typeof(CompositeMatcher<Unit>),
			typeof(EmptyMatcher<Unit>),
			typeof(InverseMatcher<Unit>),

			typeof(UnitHasConfiguration),
			typeof(UnitHasEvacuated),
			typeof(UnitHasPosition),
			typeof(UnitHasReconned),
			typeof(UnitHasStatus),
			typeof(UnitIsHostile)
		})
		{ }

		public override IEnumerable<Tuple<string, Func<ParseBlock, object>>> GetParsers(params Type[] FilterTypes)
		{
			foreach (var p in base.GetParsers(
				Enumerable.Concat(
					FilterTypes,
					new Type[]
					{
						typeof(EmptyMatcher<Tile>),
						typeof(EmptyMatcher<Unit>),
						typeof(InverseMatcher<Tile>),
						typeof(InverseMatcher<Unit>)
					}).ToArray()))
				yield return p;

			yield return new Tuple<string, Func<ParseBlock, object>>("any-tile", i => new EmptyMatcher<Tile>());
			yield return new Tuple<string, Func<ParseBlock, object>>(
				"tile-matches-all", i => new CompositeMatcher<Tile>(i, CompositeMatcher<Tile>.AND));
			yield return new Tuple<string, Func<ParseBlock, object>>(
				"tile-matches-any", i => new CompositeMatcher<Tile>(i, CompositeMatcher<Tile>.OR));
			yield return new Tuple<string, Func<ParseBlock, object>>("tile-not", i => new InverseMatcher<Tile>(i));

			yield return new Tuple<string, Func<ParseBlock, object>>("any-unit", i => new EmptyMatcher<Unit>());
			yield return new Tuple<string, Func<ParseBlock, object>>(
				"unit-matches-all", i => new CompositeMatcher<Unit>(i, CompositeMatcher<Unit>.AND));
			yield return new Tuple<string, Func<ParseBlock, object>>(
				"unit-matches-any", i => new CompositeMatcher<Unit>(i, CompositeMatcher<Unit>.OR));
			yield return new Tuple<string, Func<ParseBlock, object>>("unit-not", i => new InverseMatcher<Unit>(i));
		}
	}
}
