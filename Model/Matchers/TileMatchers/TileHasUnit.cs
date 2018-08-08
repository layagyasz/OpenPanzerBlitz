using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileHasUnit : Matcher<Tile>
	{
		enum Attribute { MATCHER };

		public readonly Matcher<Unit> Matcher;

		public override bool IsTransient { get; } = true;

		public TileHasUnit(Matcher<Unit> Matcher)
		{
			this.Matcher = Matcher;
		}

		public TileHasUnit(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Matcher = (Matcher<Unit>)attributes[(int)Attribute.MATCHER];
		}

		public TileHasUnit(SerializationInputStream Stream)
			: this((Matcher<Unit>)MatcherSerializer.Instance.Deserialize(Stream)) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
		}

		public override bool Matches(Tile Object)
		{
			if (Object == null) return false;
			return Object.Units.Any(i => Matcher.Matches(i));
		}
	}
}
