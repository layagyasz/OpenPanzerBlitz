using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class AddChatMessageAction : ChatAction
	{
		public readonly ChatMessage Message;

		public AddChatMessageAction(ChatMessage Message)
		{
			this.Message = Message;
		}

		public AddChatMessageAction(SerializationInputStream Stream)
			: this(new ChatMessage(Stream)) { }

		public bool Apply(Chat Chat)
		{
			return Chat.AddChatMessage(Message);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Message);
		}
	}
}
