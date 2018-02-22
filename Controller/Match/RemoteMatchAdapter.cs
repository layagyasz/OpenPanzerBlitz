using System.Collections.Generic;

using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class RemoteMatchAdapter : MatchAdapter
	{
		readonly Match _LocalMatch;
		readonly TCPClient _Client;
		readonly OrderSerializer _Serializer;

		public RemoteMatchAdapter(Match LocalMatch, TCPClient Client, OrderSerializer Serializer)
		{
			_LocalMatch = LocalMatch;
			_Client = Client;
			_Serializer = Serializer;
		}

		public Scenario GetScenario()
		{
			return _LocalMatch.Scenario;
		}

		public Map GetMap()
		{
			return _LocalMatch.Map;
		}

		public IEnumerable<Army> GetArmies()
		{
			return _LocalMatch.Armies;
		}

		public Turn GetTurn()
		{
			return _LocalMatch.CurrentTurn;
		}

		public OrderInvalidReason ValidateOrder(Order Order)
		{
			return (OrderInvalidReason)((ByteResponse)_Client.Call(
				new ValidateOrderRequest(Order, _Serializer)).Get()).Value;
		}

		public OrderInvalidReason ExecuteOrder(Order Order)
		{
			return (OrderInvalidReason)((ByteResponse)_Client.Call(
				new ExecuteOrderRequest(Order, _Serializer)).Get()).Value;
		}
	}
}
