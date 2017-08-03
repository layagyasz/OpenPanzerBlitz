using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public static class MatcherSerializer
	{
		static readonly Type[] MATCHER_TYPES =
		{
			typeof(CompositeMatcher),
			typeof(DistanceFromUnit),
			typeof(TileElevation),
			typeof(TileOnEdge),
			typeof(TileWithin)
		};

		static readonly Func<SerializationInputStream, Matcher>[] DESERIALIZERS =
		{
			i => new CompositeMatcher(i),
			i => new DistanceFromUnit(i),
			i => new TileElevation(i),
			i => new TileOnEdge(i),
			i => new TileWithin(i)
		};

		public static void Serialize(Matcher Matcher, SerializationOutputStream Stream)
		{
			Stream.Write((byte)Array.IndexOf(MATCHER_TYPES, Matcher.GetType()));
			Stream.Write(Matcher);
		}

		public static Matcher Deserialize(SerializationInputStream Stream)
		{
			return DESERIALIZERS[Stream.ReadByte()](Stream);
		}
	}
}