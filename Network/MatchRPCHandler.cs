using System;

using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class MatchRPCHandler : RPCHandler
	{
		Match _Match;

		public MatchRPCHandler(Match Match)
		{
			_Match = Match;
			RegisterRPC(typeof(ExecuteOrderRequest), i => ExecuteOrder((ExecuteOrderRequest)i));
			RegisterRPC(typeof(ValidateOrderRequest), i => ValidateOrder((ValidateOrderRequest)i));
		}

		RPCResponse ExecuteOrder(ExecuteOrderRequest Request)
		{
			return new BooleanResponse(_Match.BufferOrder(Request.Order));
		}

		RPCResponse ValidateOrder(ValidateOrderRequest Request)
		{
			return new BooleanResponse(_Match.ValidateOrder(Request.Order));
		}
	}
}
