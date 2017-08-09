using System;

using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class RemoteMatchAdapter : MatchAdapter
	{
		Match _LocalMatch;
		RPCAdapter _Adapter;
		OrderSerializer _Serializer;

		public RemoteMatchAdapter(Match LocalMatch, RPCAdapter Adapter)
		{
			_LocalMatch = LocalMatch;
			_Serializer = new OrderSerializer(LocalMatch.GetGameObjects());
			_Adapter = Adapter;
		}

		public Map GetMap()
		{
			return _LocalMatch.Map;
		}

		public TurnInfo GetTurnInfo()
		{
			return _LocalMatch.CurrentPhase;
		}

		public bool ValidateOrder(Order Order)
		{
			return ((BooleanResponse)_Adapter.Call(new ValidateOrderRequest(Order, _Serializer)).Get()).Value;
		}

		public bool ExecuteOrder(Order Order)
		{
			return ((BooleanResponse)_Adapter.Call(new ExecuteOrderRequest(Order, _Serializer)).Get()).Value;
		}
	}
}
