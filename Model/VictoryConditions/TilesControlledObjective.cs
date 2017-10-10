using System;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TilesControlledObjective : Objective
	{
		enum Attribute { FRIENDLY, MATCHER };

		public readonly bool Friendly;
		public readonly Matcher<Tile> Matcher;

		public TilesControlledObjective(string UniqueKey, bool Friendly, Matcher<Tile> Matcher)
			: base(UniqueKey)
		{
			this.Friendly = Friendly;
			this.Matcher = Matcher;
		}

		public TilesControlledObjective(ParseBlock Block)
			: base(Block.Name)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Friendly = (bool)attributes[(int)Attribute.FRIENDLY];
			Matcher = (Matcher<Tile>)attributes[(int)Attribute.MATCHER];
		}

		public TilesControlledObjective(SerializationInputStream Stream)
			: this(
				Stream.ReadString(),
				Stream.ReadBoolean(),
				(Matcher<Tile>)MatcherSerializer.Instance.Deserialize(Stream))
		{ }

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			Stream.Write(Friendly);
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
		}

		public override int CalculateScore(Army ForArmy, Match Match)
		{
			_Score = Match.Map.TilesEnumerable.Count(
				i => Friendly == (i.ControllingArmy == ForArmy) && Matcher.Matches(i));
			return _Score;
		}
	}
}
