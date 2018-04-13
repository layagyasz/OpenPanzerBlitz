using System;

namespace PanzerBlitz
{
	public class AIRoot
	{
		public readonly MatchAdapter Match;
		public readonly Army Army;

		public readonly UnitAssigner UnitAssignments = new UnitAssigner();
		public readonly TileEvaluator TileEvaluations;

		public readonly Random Random = new Random();

		public AIRoot(MatchAdapter Match, Army Army)
		{
			this.Match = Match;
			this.Army = Army;

			TileEvaluations = new TileEvaluator(Match, Army);
		}

		public void ReAssign()
		{
			UnitAssignments.ClearAssignments();
			foreach (var deployment in Army.Deployments) UnitAssignments.MakeAssignments(deployment);
		}
	}
}
