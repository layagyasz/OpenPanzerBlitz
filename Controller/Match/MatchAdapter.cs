using System;
namespace PanzerBlitz
{
	public interface MatchAdapter
	{
		Map GetMap();
		TurnInfo GetTurnInfo();
		OrderInvalidReason ValidateOrder(Order Order);
		OrderInvalidReason ExecuteOrder(Order Order);
	}
}
