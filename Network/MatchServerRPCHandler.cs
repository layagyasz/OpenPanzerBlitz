using System.Collections.Generic;

using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class MatchServerRPCHandler : MatchRPCHandler
	{
		Dictionary<Army, TCPConnection> _ArmyConnections;

		public MatchServerRPCHandler(Match Match, Dictionary<Army, TCPConnection> ArmyConnections)
			: base(Match)
		{
			_ArmyConnections = ArmyConnections;
		}

		bool ArmyMatches(Army Army, TCPConnection Connection)
		{
			return _ArmyConnections.ContainsKey(Army) && _ArmyConnections[Army] == Connection;
		}

		protected override RPCResponse ExecuteOrder(ExecuteOrderRequest Request, TCPConnection Connection)
		{
			if (ArmyMatches(Request.Order.Army, Connection)) return base.ExecuteOrder(Request, Connection);
			return new BooleanResponse(false);
		}

		protected override RPCResponse ValidateOrder(ValidateOrderRequest Request, TCPConnection Connection)
		{
			if (ArmyMatches(Request.Order.Army, Connection)) return base.ValidateOrder(Request, Connection);
			return new BooleanResponse(false);
		}
	}
}
