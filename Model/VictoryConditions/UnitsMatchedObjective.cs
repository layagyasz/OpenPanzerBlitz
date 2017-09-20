using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitsMatchedObjective : Objective
	{
		enum Attribute { FRIENDLY, MATCHER };

		public readonly Matcher<Unit> Matcher;
		public readonly bool Friendly;

		public UnitsMatchedObjective(string UniqueKey, Matcher<Unit> Matcher, bool Friendly)
			: base(UniqueKey)
		{
			this.Matcher = Matcher;
			this.Friendly = Friendly;
		}

		public UnitsMatchedObjective(ParseBlock Block)
			: base(Block.Name)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Matcher = (Matcher<Unit>)attributes[(int)Attribute.MATCHER];
			Friendly = (bool)attributes[(int)Attribute.FRIENDLY];
		}

		public UnitsMatchedObjective(SerializationInputStream Stream)
			: this(
				Stream.ReadString(),
				(Matcher<Unit>)MatcherSerializer.Instance.Deserialize(Stream),
				Stream.ReadBoolean())
		{ }

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
			Stream.Write(Friendly);
		}

		public override int CalculateScore(Army ForArmy, Match Match)
		{
			_Score = Match.Armies.Where(i => Friendly == (i.Configuration.Team == ForArmy.Configuration.Team))
						  .SelectMany(i => i.Units).Where(Matcher.Matches).Count();
			return _Score;
		}
	}
}
