using System.Collections.Generic;

namespace PanzerBlitz
{
	public class MultiTurnOrderAutomater : OrderAutomater
	{
		readonly Dictionary<TurnInfo, List<Order>> _RecurringOrderBuffer = new Dictionary<TurnInfo, List<Order>>();

		public void Hook(EventRelay Relay)
		{
			return;
		}

		public bool AutomateTurn(Match Match, TurnInfo TurnInfo)
		{
			DoBufferedOrders(Match, TurnInfo);
			return false;
		}

		public void BufferOrder(Order Order, TurnInfo TurnInfo)
		{
			if (!_RecurringOrderBuffer.ContainsKey(TurnInfo)) _RecurringOrderBuffer.Add(TurnInfo, new List<Order>());

			List<Order> orders = _RecurringOrderBuffer[TurnInfo];
			if (!orders.Contains(Order)) orders.Add(Order);
		}

		void DoBufferedOrders(Match Match, TurnInfo TurnInfo)
		{
			if (_RecurringOrderBuffer.ContainsKey(TurnInfo))
			{
				List<Order> orders = _RecurringOrderBuffer[TurnInfo];
				_RecurringOrderBuffer.Remove(TurnInfo);
				orders.ForEach(i => Match.ExecuteOrder(i));
			}
		}
	}
}
