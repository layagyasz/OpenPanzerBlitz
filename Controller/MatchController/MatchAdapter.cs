using System;
namespace PanzerBlitz
{
	public interface MatchAdapter
	{
		Map GetMap();
		TurnInfo GetTurnInfo();
		bool ValidateOrder(Order Order);
		bool ExecuteOrder(Order Order);
	}
}
