using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class ChatServerLayer : ChatLayer
	{
		readonly ConnectionCache<Player> _PlayerConnections;

		public ChatServerLayer(Chat Chat, ConnectionCache<Player> PlayerConnections)
			: base(Chat)
		{
			_PlayerConnections = PlayerConnections;
		}

		public override RPCResponse ApplyChatAction(ApplyChatActionRequest Request, TCPConnection Connection)
		{
			if (_PlayerConnections.PlayerMatches(Request.Action.Player, Connection))
				return base.ApplyChatAction(Request, Connection);
			return new BooleanResponse(false);
		}
	}
}
