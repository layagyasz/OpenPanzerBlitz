using System;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitsInZoneObjective : Objective
	{
		enum Attribute { MATCHER, FRIENDLY }

		public readonly Matcher Matcher;
		public readonly bool Friendly;

		public UnitsInZoneObjective(string UniqueKey, Matcher Matcher, bool Friendly)
			: base(UniqueKey)
		{
			this.Matcher = Matcher;
			this.Friendly = Friendly;
		}

		public UnitsInZoneObjective(ParseBlock Block)
			: base(Block.Name)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Matcher = (Matcher)attributes[(int)Attribute.MATCHER];
			Friendly = Parse.DefaultIfNull(attributes[(int)Attribute.FRIENDLY], true);
		}

		public UnitsInZoneObjective(SerializationInputStream Stream)
			: this(Stream.ReadString(), MatcherSerializer.Deserialize(Stream), Stream.ReadBoolean()) { }

		public override void Serialize(SerializationOutputStream Stream)
		{
			base.Serialize(Stream);
			MatcherSerializer.Serialize(Matcher, Stream);
			Stream.Write(Friendly);
		}

		public override int CalculateScore(Army ForArmy, Match Match)
		{
			_Score = Match.Armies.Where(
				i => Friendly == (i.Configuration.Team == ForArmy.Configuration.Team))
						  .SelectMany(i => i.Units)
						  .Count(i => i.Position != null
								 && !i.Configuration.IsNeutral()
								 && (Matcher == null || Matcher.Matches(i.Position)));
			return _Score;
		}
	}
}
