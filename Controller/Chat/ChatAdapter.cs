using System;
namespace PanzerBlitz
{
	public interface ChatAdapter
	{
		bool SendMessage(ChatMessage Message);
	}
}
