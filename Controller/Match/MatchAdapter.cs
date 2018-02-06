using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public interface MatchAdapter
	{
		Scenario GetScenario();
		Map GetMap();
		IEnumerable<Army> GetArmies();
		Turn GetTurn();
		OrderInvalidReason ValidateOrder(Order Order);
		OrderInvalidReason ExecuteOrder(Order Order);
	}
}
