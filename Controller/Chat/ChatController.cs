using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class ChatController
	{
		readonly ChatAdapter _Adapter;
		readonly ChatView _ChatView;
		readonly Player _Player;

		public ChatController(ChatAdapter Adapter, ChatView ChatView, Player Player)
		{
			_Adapter = Adapter;
			_ChatView = ChatView;
			_ChatView.OnTextSubmitted += HandleSendMessage;
			_Player = Player;
		}

		void HandleSendMessage(object Sender, ValuedEventArgs<string> E)
		{
			if (E.Value != "") _Adapter.SendMessage(new ChatMessage(_Player, E.Value));
		}
	}
}
