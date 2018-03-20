using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileDistanceFrom : Matcher<Tile>
	{
		enum Attribute { MATCHER, DISTANCE, ATLEAST };

		public readonly Matcher<Tile> Matcher;
		public readonly byte Distance;
		public readonly bool Atleast;

		public TileDistanceFrom(Matcher<Tile> Matcher, byte Distance, bool Atleast)
		{
			this.Matcher = Matcher;
			this.Distance = Distance;
			this.Atleast = Atleast;
		}

		public TileDistanceFrom(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Matcher = (Matcher<Tile>)attributes[(int)Attribute.MATCHER];
			Distance = (byte)attributes[(int)Attribute.DISTANCE];
			Atleast = (bool)(attributes[(int)Attribute.ATLEAST] ?? false);
		}

		public TileDistanceFrom(SerializationInputStream Stream)
			: this(
				(Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream),
				Stream.ReadByte(),
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

			return new Field<Tile>(Tile, Distance - (Atleast ? 1 : 0), (i, j) => 1)
				.GetReachableNodes()
				.Any(i => Matcher.Matches(i.Item1)) ^ Atleast;
		}

		public IEnumerable<Matcher<Tile>> Flatten()
		{
			yield return this;
		}
	}
}
