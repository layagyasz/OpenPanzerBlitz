using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class LocalMatchAdapter : MatchAdapter
	{
		public readonly Match Match;

		public LocalMatchAdapter(Match Match)
		{
			this.Match = Match;
		}

		public Scenario GetScenario()
		{
			return Match.Scenario;
		}

		public Map GetMap()
		{
			return Match.Map;
		}

		public IEnumerable<Army> GetArmies()
		{
			return Match.Armies;
		}

		public Turn GetTurn()
		{
			return Match.CurrentTurn;
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
