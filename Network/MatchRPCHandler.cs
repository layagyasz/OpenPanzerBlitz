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
			RegisterRPC(typeof(ExecuteOrderRequest), (i, j) => ExecuteOrder((ExecuteOrderRequest)i, j));
			RegisterRPC(typeof(ValidateOrderRequest), (i, j) => ValidateOrder((ValidateOrderRequest)i, j));
		}

		protected virtual RPCResponse ExecuteOrder(ExecuteOrderRequest Request, TCPConnection Connection)
		{
			return new BooleanResponse(_Match.BufferOrder(Request.Order));
		}

		protected virtual RPCResponse ValidateOrder(ValidateOrderRequest Request, TCPConnection Connection)
		{
			return new ByteResponse((byte)_Match.ValidateOrder(Request.Order));
		}
	}
}
