using System;
using System.Collections.Generic;

using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class RemotePlayerController : GamePlayerController
	{
		Match _Match;
		RPCAdapter _Adapter;
		OrderSerializer _OrderSerializer;

		public RemotePlayerController(Match Match, TCPConnection Connection)
		{
			_Match = Match;
			_OrderSerializer = new OrderSerializer(Match.GetGameObjects());
			_Adapter = new RPCAdapter(Connection, new Dictionary<Type, Func<RPCRequest, RPCResponse>>()
			{
				{ typeof(ValidateOrderRequest), i => HandleValidateOrderRequest((ValidateOrderRequest)i) },
				{ typeof(ExecuteOrderRequest), i => HandleExecuteOrderRequest((ExecuteOrderRequest)i) }
			});
		}

		public void DoTurn(TurnInfo TurnInfo)
		{
			_Adapter.Call(new DoTurnRequest(TurnInfo));
		}

		RPCResponse HandleValidateOrderRequest(ValidateOrderRequest Request)
		{
			return new BooleanResponse(_Match.ValidateOrder(Request.Order));
		}

		RPCResponse HandleExecuteOrderRequest(ExecuteOrderRequest Request)
		{
			return new BooleanResponse(_Match.ExecuteOrder(Request.Order));
		}
	}
}
