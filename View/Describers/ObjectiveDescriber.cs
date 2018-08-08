using System.Linq;

namespace PanzerBlitz
{
	public static class ObjectiveDescriber
	{
		public static string Describe(Objective Objective)
		{
			if (Objective is CompositeObjective) return Describe((CompositeObjective)Objective);
			if (Objective is FurthestAdvanceObjective) return Describe((FurthestAdvanceObjective)Objective);
			if (Objective is HighestScoreObjective) return Describe((HighestScoreObjective)Objective);
			if (Objective is HighestUniqueScoreObjective) return Describe((HighestUniqueScoreObjective)Objective);
			if (Objective is LineOfFireObjective) return Describe((LineOfFireObjective)Objective);
			if (Objective is PathObjective) return Describe((PathObjective)Objective);
			if (Objective is PointsObjective) return Describe((PointsObjective)Objective);
			if (Objective is PreventEnemyObjective) return Describe((PreventEnemyObjective)Objective);
			if (Objective is RatioObjective) return Describe((RatioObjective)Objective);
			if (Objective is SumObjective) return Describe((SumObjective)Objective);
			if (Objective is TemporalObjective) return Describe((TemporalObjective)Objective);
			if (Objective is TilesControlledObjective) return Describe((TilesControlledObjective)Objective);
			if (Objective is TriggerObjective) return Describe((TriggerObjective)Objective);
			if (Objective is UnitsMatchedObjective) return Describe((UnitsMatchedObjective)Objective);

			return string.Empty;
		}

		public static string Describe(CompositeObjective Objective)
		{
			return ObjectDescriber.Listify(
				Objective.Objectives.Select(i => ReplaceScore(Describe(i), "number of")),
				", ",
				Objective.Aggregator == Aggregators.AND ? ", and " : ", or ");
		}

		public static string Describe(FurthestAdvanceObjective Objective)
		{
			return string.Format("%score% hexes advanced {0}", ObjectDescriber.Describe(Objective.Direction));
		}

		public static string Describe(HighestScoreObjective Objective)
		{
			return string.Format("the highest {0}", ReplaceScore(Describe(Objective.Metric), "number of"));
		}

		public static string Describe(HighestUniqueScoreObjective Objective)
		{
			return "highest total score";
		}

		public static string Describe(LineOfFireObjective Objective)
		{
			return string.Format(
				"{0} {1} {2} {3} hexes wide from {4} to {5}",
				Objective.BreakThrough ? "create a corridor clear of" : "establish an unbroken line of",
				Objective.Friendly ? "friendly" : "enemy",
				Objective.IncludeFieldOfSight ? "fire" : "units",
				Objective.Width,
				ObjectDescriber.Describe(Objective.Vertical ? Direction.NORTH : Direction.EAST),
				ObjectDescriber.Describe(Objective.Vertical ? Direction.SOUTH : Direction.WEST));
		}

		public static string Describe(PathObjective Objective)
		{
			return string.Format(
				"establish a path from tiles {0} to tiles {1} through tiles {2}",
				MatcherDescriber.Describe(Objective.Source),
				MatcherDescriber.Describe(Objective.Sink),
				MatcherDescriber.Describe(Objective.Path));
		}

		public static string Describe(PointsObjective Objective)
		{
			return Describe(Objective.Objective)
				+ (Objective.Points > 1 ? " times " + Objective.Points.ToString() : "");
		}

		public static string Describe(PreventEnemyObjective Objective)
		{
			return "prevent enemy victory conditions";
		}

		public static string Describe(RatioObjective Objective)
		{
			return string.Format(
				"ratio of {0} to {1} is %score% to 1",
				ReplaceScore(Describe(Objective.Numerator), "number of"),
				ReplaceScore(Describe(Objective.Denominator), "number of"));
		}

		public static string Describe(SumObjective Objective)
		{
			return string.Format(
				"sum of {0} is %score%",
				ObjectDescriber.Listify(
					Objective.Objectives.Select(i => ReplaceScore(Describe(i), "number of")), ", ", ", and "));
		}

		public static string Describe(TemporalObjective Objective)
		{
			return string.Format("{0} by turn %score%", Describe(Objective.Objective));
		}

		public static string Describe(TilesControlledObjective Objective)
		{
			return string.Format(
				"%score% tiles {0} under {1} control",
				MatcherDescriber.Describe(Objective.Matcher),
				Objective.Friendly ? "friendly" : "enemy");
		}

		public static string Describe(TriggerObjective Objective)
		{
			return ReplaceScore(
				Describe(Objective.Objective),
				(Objective.Invert ? "at most " : "at least ") + Objective.Threshold.ToString());
		}

		public static string Describe(UnitsMatchedObjective Objective)
		{
			return string.Format(
				"%score% {0}{1} units {2}",
				Objective.CountPoints ? "points of " : "",
				Objective.Friendly ? "friendly" : "enemy",
				MatcherDescriber.Describe(Objective.Matcher));
		}

		public static string ReplaceScore(string Input, string Value)
		{
			return Input.Replace("%score%", Value);
		}

		public static string RemoveScore(string Input)
		{
			return Input.Replace("%score%", "number of");
		}
	}
}
