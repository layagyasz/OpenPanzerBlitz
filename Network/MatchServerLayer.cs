using System.Collections.Generic;

using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class MatchServerLayer : MatchLayer
	{
		readonly Dictionary<Army, Player> _PlayerMap;
		readonly ConnectionCache<Player> _PlayerConnections;

		public MatchServerLayer(Match Match, Dictionary<Army, Player> PlayerMap, ConnectionCache<Player> PlayerConnections)
			: base(Match)
		{
			_PlayerMap = PlayerMap;
			_PlayerConnections = PlayerConnections;
		}

		public override RPCResponse ExecuteOrder(ExecuteOrderRequest Request, TCPConnection Connection)
		{
			if (_PlayerConnections.PlayerMatches(_PlayerMap[Request.Order.Army], Connection))
				return base.ExecuteOrder(Request, Connection);
			return new ByteResponse((byte)OrderInvalidReason.ILLEGAL);
		}

		public override RPCResponse ValidateOrder(ValidateOrderRequest Request, TCPConnection Connection)
		{
			if (_PlayerConnections.PlayerMatches(_PlayerMap[Request.Order.Army], Connection))
				return base.ValidateOrder(Request, Connection);
			return new ByteResponse((byte)OrderInvalidReason.ILLEGAL);
		}
	}
}
