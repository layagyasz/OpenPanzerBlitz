using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitHasEvacuated : Matcher<Unit>
	{
		enum Attribute { DIRECTION, MATCHER };

		public readonly Direction Direction;
		public readonly Matcher<Tile> Matcher;

		public override bool IsTransient { get; } = true;

		public UnitHasEvacuated(Direction Direction, Matcher<Tile> Matcher)
		{
			this.Direction = Direction;
			this.Matcher = Matcher;
		}

		public UnitHasEvacuated(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Direction = (Direction)attributes[(int)Attribute.DIRECTION];
			Matcher = (Matcher<Tile>)(attributes[(int)Attribute.MATCHER] ?? new EmptyMatcher<Tile>());
		}

		public UnitHasEvacuated(SerializationInputStream Stream)
			: this((Direction)Stream.ReadByte(), (Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream))
		{ }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)Direction);
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
		}

		public override bool Matches(Unit Object)
		{
			if (Object.Evacuated == null) return false;
			return Matcher.Matches(Object.Evacuated) && Object.Evacuated.OnEdge(Direction);
		}
	}
}
