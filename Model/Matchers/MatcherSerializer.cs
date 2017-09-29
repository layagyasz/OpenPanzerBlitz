using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class MatcherSerializer : SerializableAdapter
	{
		public static readonly MatcherSerializer Instance = new MatcherSerializer();

		MatcherSerializer()
			: base(new Tuple<Type, Func<SerializationInputStream, Serializable>>[]
		{
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(EmptyMatcher<Tile>), i => new EmptyMatcher<Tile>(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(InverseMatcher<Tile>), i => new InverseMatcher<Tile>(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(CompositeMatcher<Tile>), i => new CompositeMatcher<Tile>(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(TileDistanceFromUnit), i => new TileDistanceFromUnit(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(TileElevation), i => new TileElevation(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(TileOnEdge), i => new TileOnEdge(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(TileWithin), i => new TileWithin(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(TileHasCoordinate), i => new TileHasCoordinate(i)),

			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(EmptyMatcher<Unit>), i => new EmptyMatcher<Unit>(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(InverseMatcher<Unit>), i => new InverseMatcher<Unit>(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(CompositeMatcher<Unit>), i => new CompositeMatcher<Unit>(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(UnitHasStatus), i => new UnitHasStatus(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(UnitHasReconned), i => new UnitHasReconned(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(UnitHasEvacuated), i => new UnitHasEvacuated(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(UnitHasPosition), i => new UnitHasPosition(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(UnitHasConfiguration), i => new UnitHasConfiguration(i)),
			new Tuple<Type, Func<SerializationInputStream, Serializable>>(
				typeof(UnitIsHostile), i => new UnitIsHostile(i))
		})
		{ }
	}
}
