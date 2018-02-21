using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitHasEvacuated : Matcher<Unit>
	{
		enum Attribute { DIRECTION, MATCHER };

		public readonly Direction Direction;
		public readonly Matcher<Tile> Matcher;

		public UnitHasEvacuated(Direction Direction, Matcher<Tile> Matcher)
		{
			this.Direction = Direction;
			this.Matcher = Matcher;
		}

		public UnitHasEvacuated(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Direction = (Direction)attributes[(int)Attribute.DIRECTION];
			Matcher = Parse.DefaultIfNull<Matcher<Tile>>(attributes[(int)Attribute.MATCHER], new EmptyMatcher<Tile>());
		}

		public UnitHasEvacuated(SerializationInputStream Stream)
			: this((Direction)Stream.ReadByte(), (Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream))
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)Direction);
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
		}

		public bool Matches(Unit Unit)
		{
			if (Unit.Evacuated == null) return false;
			return Matcher.Matches(Unit.Evacuated) && Unit.Evacuated.OnEdge(Direction);
		}

		public IEnumerable<Matcher<Unit>> Flatten()
		{
			yield return this;
		}
	}
}
