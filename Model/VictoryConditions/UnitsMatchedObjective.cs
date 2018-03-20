using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitsMatchedObjective : Objective
	{
		enum Attribute { FRIENDLY, MATCHER, COUNT_POINTS };

		public readonly Matcher<Unit> Matcher;
		public readonly bool Friendly;
		public readonly bool CountPoints;

		public UnitsMatchedObjective(Matcher<Unit> Matcher, bool Friendly, bool CountPoints)
		{
			this.Matcher = Matcher;
			this.Friendly = Friendly;
			this.CountPoints = CountPoints;
		}

		public UnitsMatchedObjective(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Matcher = (Matcher<Unit>)attributes[(int)Attribute.MATCHER];
			Friendly = (bool)attributes[(int)Attribute.FRIENDLY];
			CountPoints = (bool)(attributes[(int)Attribute.COUNT_POINTS] ?? false);
		}

		public UnitsMatchedObjective(SerializationInputStream Stream)
			: this(
				(Matcher<Unit>)MatcherSerializer.Instance.Deserialize(Stream),
				Stream.ReadBoolean(),
				Stream.ReadBoolean())
		{ }

		public override void Serialize(SerializationOutputStream Stream)
		{
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
			Stream.Write(Friendly);
			Stream.Write(CountPoints);
		}

		public override bool CanStopEarly()
		{
			return false;
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			return (int)Match.Armies
							.Where(i => Friendly == (i.Configuration.Team == ForArmy.Configuration.Team))
							.SelectMany(i => i.Units)
							.Where(Matcher.Matches)
							.Sum(i => CountPoints ? i.GetPointValue() * 100 : 1);
		}

		public override IEnumerable<Tile> GetTiles(Map Map)
		{
			return Matcher.Flatten()
						  .Where(i => i is UnitHasPosition)
						  .Cast<UnitHasPosition>()
						  .SelectMany(i => Map.TilesEnumerable.Where(j => i.Matcher.Matches(j)));
		}
	}
}
