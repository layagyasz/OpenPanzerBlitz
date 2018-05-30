using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class ChatLayer : RPCHandlerLayer
	{
		public Chat Chat { get; }

		public ChatLayer(Chat Chat)
		{
			this.Chat = Chat;
		}

		public virtual RPCResponse ApplyChatAction(ApplyChatActionRequest Request, TCPConnection Connection)
		{
			return new BooleanResponse(Chat != null && Chat.ApplyAction(Request.Action));
		}

		public void Install(RPCHandler Handler)
		{
			Handler.RegisterRPC<ApplyChatActionRequest>(ApplyChatAction);
		}
	}
}
