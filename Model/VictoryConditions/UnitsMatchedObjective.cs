using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnitsMatchedObjective : Objective
	{
		enum Attribute { FRIENDLY, MATCHER, COUNT_POINTS, POINT_VALUE };

		public readonly Matcher<Unit> Matcher;
		public readonly bool Friendly;
		public readonly bool CountPoints;
		public readonly int PointValue;

		public UnitsMatchedObjective(Matcher<Unit> Matcher, bool Friendly, bool CountPoints, int PointValue)
		{
			this.Matcher = Matcher;
			this.Friendly = Friendly;
			this.CountPoints = CountPoints;
			this.PointValue = PointValue;
		}

		public UnitsMatchedObjective(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Matcher = (Matcher<Unit>)attributes[(int)Attribute.MATCHER];
			Friendly = (bool)attributes[(int)Attribute.FRIENDLY];
			CountPoints = Parse.DefaultIfNull(attributes[(int)Attribute.COUNT_POINTS], false);
			PointValue = Parse.DefaultIfNull(attributes[(int)Attribute.POINT_VALUE], 1);
		}

		public UnitsMatchedObjective(SerializationInputStream Stream)
			: this(
				(Matcher<Unit>)MatcherSerializer.Instance.Deserialize(Stream),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadInt32())
		{ }

		public override void Serialize(SerializationOutputStream Stream)
		{
			MatcherSerializer.Instance.Serialize(Matcher, Stream);
			Stream.Write(Friendly);
			Stream.Write(CountPoints);
			Stream.Write(PointValue);
		}

		public override bool CanStopEarly()
		{
			return false;
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			return PointValue * (int)Match.Armies
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
