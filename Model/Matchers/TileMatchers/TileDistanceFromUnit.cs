using System;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileDistanceFromUnit : Matcher<Tile>
	{
		enum Attribute { MATCHER, DISTANCE, ATLEAST };

		public readonly Matcher<Unit> Matcher;
		public readonly int Distance;
		public readonly bool Atleast;

		public TileDistanceFromUnit(Matcher<Unit> Matcher, int Distance, bool Atleast)
		{
			this.Matcher = Matcher;
			this.Distance = Distance;
			this.Atleast = Atleast;
		}

		public TileDistanceFromUnit(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Matcher = (Matcher<Unit>)attributes[(int)Attribute.MATCHER];
			Distance = (int)attributes[(int)Attribute.DISTANCE];
			Atleast = Parse.DefaultIfNull(attributes[(int)Attribute.ATLEAST], false);
		}

		public TileDistanceFromUnit(SerializationInputStream Stream)
			: this(
				(Matcher<Unit>)MatcherSerializer.Instance.Deserialize(Stream),
				Stream.ReadInt32(),
				Stream.ReadBoolean())
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
			Stream.Write(Distance);
			Stream.Write(Atleast);
		}

		public bool Matches(Tile Tile)
		{
			if (Tile == null) return false;

			return new Field<Tile>(Tile, Distance, (i, j) => 1)
				.GetReachableNodes()
				.Any(i => i.Item1.Units.Any(j => Matcher.Matches(j))) ^ Atleast;
		}
	}
}
