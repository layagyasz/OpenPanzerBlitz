namespace PanzerBlitz
{
	public class LocalChatAdapter : ChatAdapter
	{
		readonly Chat _Chat;

		public LocalChatAdapter(Chat Chat)
		{
			_Chat = Chat;
		}

		public bool SendMessage(ChatMessage Message)
		{
			return _Chat.ApplyAction(new AddChatMessageAction(Message));
		}
	}
}
