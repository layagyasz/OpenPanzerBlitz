using System;
namespace PanzerBlitz
{
	public class LocalMatchAdapter : MatchAdapter
	{
		public readonly Match Match;

		public LocalMatchAdapter(Match Match)
		{
			this.Match = Match;
		}

		public Map GetMap()
		{
			return Match.Map;
		}

		public TurnInfo GetTurnInfo()
		{
			return Match.CurrentPhase;
		}

		public bool ValidateOrder(Order Order)
		{
			return Match.ValidateOrder(Order);
		}

		public bool ExecuteOrder(Order Order)
		{
			return Match.ExecuteOrder(Order);
		}
	}
}
