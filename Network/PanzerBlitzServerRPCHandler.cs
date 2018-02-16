using Cardamom.Network;

namespace PanzerBlitz
{
	public class PanzerBlitzServerRPCHandler : RPCHandler
	{
		readonly PanzerBlitzServer _Server;

		public PanzerBlitzServerRPCHandler(PanzerBlitzServer Server)
		{
			_Server = Server;
			RegisterRPC(typeof(LogInPlayerRequest), (i, j) => LogInPlayer((LogInPlayerRequest)i));
			RegisterRPC(typeof(RegisterPlayerRequest), (i, j) => RegisterPlayer((RegisterPlayerRequest)i));
		}

		public LogInPlayerResponse LogInPlayer(LogInPlayerRequest Request)
		{
			return new LogInPlayerResponse(_Server.LogInPlayer(Request.Username, Request.Password));
		}

		public LogInPlayerResponse RegisterPlayer(RegisterPlayerRequest Request)
		{
			return new LogInPlayerResponse(_Server.RegisterPlayer(Request.Username, Request.Password));
		}
	}
}
