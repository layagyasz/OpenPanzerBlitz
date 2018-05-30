using Cardamom.Network;

namespace PanzerBlitz
{
	public class PanzerBlitzServerLayer : RPCHandlerLayer
	{
		public readonly PanzerBlitzServer Server;

		public PanzerBlitzServerLayer(PanzerBlitzServer Server)
		{
			this.Server = Server;
		}

		public LogInPlayerResponse LogInPlayer(LogInPlayerRequest Request, TCPConnection Connection)
		{
			return new LogInPlayerResponse(Server.LogInPlayer(Request.Username, Request.Password));
		}

		public LogInPlayerResponse RegisterPlayer(RegisterPlayerRequest Request, TCPConnection Connection)
		{
			return new LogInPlayerResponse(Server.RegisterPlayer(Request.Username, Request.Password));
		}

		public void Install(RPCHandler Handler)
		{
			Handler.RegisterRPC<LogInPlayerRequest>(LogInPlayer);
			Handler.RegisterRPC<RegisterPlayerRequest>(RegisterPlayer);
		}
	}
}
