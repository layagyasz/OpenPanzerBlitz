using System;

namespace PanzerBlitz
{
	public class AIRoot
	{
		public readonly MatchAdapter Match;
		public readonly Army Army;

		public readonly UnitAssigner UnitAssignments;
		public readonly TileEvaluator TileEvaluations;

		public readonly Random Random = new Random();

		public AIRoot(MatchAdapter Match, Army Army)
		{
			this.Match = Match;
			this.Army = Army;

			UnitAssignments = new UnitAssigner(this);
			TileEvaluations = new TileEvaluator(this);
		}
	}
}
