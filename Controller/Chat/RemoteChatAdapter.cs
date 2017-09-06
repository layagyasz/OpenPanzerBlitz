using System;

using Cardamom.Network;
using Cardamom.Network.Responses;

namespace PanzerBlitz
{
	public class RemoteChatAdapter : ChatAdapter
	{
		TCPClient _Client;

		public RemoteChatAdapter(TCPClient Client)
		{
			_Client = Client;
		}

		public bool SendMessage(ChatMessage Message)
		{
			return ((BooleanResponse)_Client.Call(
				new ApplyChatActionRequest(new AddChatMessageAction(Message))).Get()).Value;
		}
	}
}