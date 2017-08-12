using System;

using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class RemoteMatchAdapter : MatchAdapter
	{
		Match _LocalMatch;
		TCPClient _Client;
		OrderSerializer _Serializer;

		public RemoteMatchAdapter(Match LocalMatch, TCPClient Client, OrderSerializer Serializer)
		{
			_LocalMatch = LocalMatch;
			_Client = Client;
			_Serializer = Serializer;
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
			return ((BooleanResponse)_Client.Call(new ValidateOrderRequest(Order, _Serializer)).Get()).Value;
		}

		public bool ExecuteOrder(Order Order)
		{
			return ((BooleanResponse)_Client.Call(new ExecuteOrderRequest(Order, _Serializer)).Get()).Value;
		}
	}
}
