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

		public OrderInvalidReason ValidateOrder(Order Order)
		{
			return Match.ValidateOrder(Order);
		}

		public OrderInvalidReason ExecuteOrder(Order Order)
		{
			return Match.ExecuteOrder(Order);
		}
	}
}
