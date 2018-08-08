using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitHasPosition : Matcher<Unit>
	{
		enum Attribute { MATCHER }

		public readonly Matcher<Tile> Matcher;

		public override bool IsTransient { get; } = true;

		public UnitHasPosition(Matcher<Tile> Matcher)
		{
			this.Matcher = Matcher;
		}

		public UnitHasPosition(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Matcher = (Matcher<Tile>)attributes[(int)Attribute.MATCHER];
		}

		public UnitHasPosition(SerializationInputStream Stream)
			: this((Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream)) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
		}

		public override bool Matches(Unit Object)
		{
			if (Object == null) return false;
			return Matcher.Matches(Object.Position);
		}
	}
}
