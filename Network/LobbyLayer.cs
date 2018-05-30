using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class LobbyLayer : RPCHandlerLayer
	{
		public MatchLobby Lobby { get; set; }

		public virtual RPCResponse ApplyLobbyAction(ApplyLobbyActionRequest Request, TCPConnection Connection)
		{
			return new BooleanResponse(Lobby != null && Lobby.ApplyAction(Request.Action));
		}

		public virtual RPCResponse GetLobby(GetLobbyRequest Request, TCPConnection Connection)
		{
			return new GetLobbyResponse(Lobby);
		}

		public void Install(RPCHandler Handler)
		{
			Handler.RegisterRPC<ApplyLobbyActionRequest>(ApplyLobbyAction);
			Handler.RegisterRPC<GetLobbyRequest>(GetLobby);
		}
	}
}
