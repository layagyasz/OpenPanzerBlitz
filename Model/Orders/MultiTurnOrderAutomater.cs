using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class MultiTurnOrderAutomater : OrderAutomater
	{
		public static readonly Func<Match, OrderAutomater> PROVIDER = i => new MultiTurnOrderAutomater(i);

		public readonly Match Match;

		readonly Dictionary<TurnInfo, List<Order>> _RecurringOrderBuffer = new Dictionary<TurnInfo, List<Order>>();

		public MultiTurnOrderAutomater(Match Match)
		{
			this.Match = Match;
		}

		public bool AutomateTurn(TurnInfo TurnInfo)
		{
			DoBufferedOrders(TurnInfo);
			return false;
		}

		public void BufferOrder(Order Order, TurnInfo TurnInfo)
		{
			if (!_RecurringOrderBuffer.ContainsKey(TurnInfo)) _RecurringOrderBuffer.Add(TurnInfo, new List<Order>());

			List<Order> orders = _RecurringOrderBuffer[TurnInfo];
			if (!orders.Contains(Order)) orders.Add(Order);
		}

		void DoBufferedOrders(TurnInfo TurnInfo)
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
