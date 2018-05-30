using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class LobbyServerLayer : LobbyLayer
	{
		readonly ConnectionCache<Player> _PlayerConnections;

		public LobbyServerLayer(MatchLobby Lobby, ConnectionCache<Player> PlayerConnections)
		{
			this.Lobby = Lobby;
			_PlayerConnections = PlayerConnections;
		}

		public override RPCResponse ApplyLobbyAction(ApplyLobbyActionRequest Request, TCPConnection Connection)
		{
			if (Request.Action is AddPlayerAction)
			{
				if (!_PlayerConnections.CanAdd(Request.Action.Player, Connection)) return new BooleanResponse(false);
				var r = (BooleanResponse)base.ApplyLobbyAction(Request, Connection);
				if (r.Value) _PlayerConnections.Add(Request.Action.Player, Connection);
				return r;
			}
			if (_PlayerConnections.PlayerMatches(Request.Action.Player, Connection))
				return base.ApplyLobbyAction(Request, Connection);
			return new BooleanResponse(false);
		}
	}
}
