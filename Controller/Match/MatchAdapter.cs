using System;
namespace PanzerBlitz
{
	public interface MatchAdapter
	{
		Scenario GetScenario();
		Map GetMap();
		Turn GetTurn();
		OrderInvalidReason ValidateOrder(Order Order);
		OrderInvalidReason ExecuteOrder(Order Order);
	}
}
