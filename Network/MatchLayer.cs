using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class MatchLayer : RPCHandlerLayer
	{
		readonly Match _Match;

		public MatchLayer(Match Match)
		{
			_Match = Match;
		}

		public virtual RPCResponse ExecuteOrder(ExecuteOrderRequest Request, TCPConnection Connection)
		{
			return new ByteResponse((byte)_Match.BufferOrder(Request.Order));
		}

		public virtual RPCResponse ValidateOrder(ValidateOrderRequest Request, TCPConnection Connection)
		{
			return new ByteResponse((byte)_Match.ValidateOrder(Request.Order));
		}

		public void Install(RPCHandler Handler)
		{
			Handler.RegisterRPC<ExecuteOrderRequest>(ExecuteOrder);
			Handler.RegisterRPC<ValidateOrderRequest>(ValidateOrder);
		}
	}
}
