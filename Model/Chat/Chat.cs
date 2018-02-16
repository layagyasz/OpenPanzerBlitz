using System;
using System.Collections.Generic;

using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class Chat
	{
		public EventHandler<ValuedEventArgs<ChatAction>> OnActionApplied;

		readonly List<ChatMessage> _Messages = new List<ChatMessage>();

		public IEnumerable<ChatMessage> Messages
		{
			get
			{
				return _Messages;
			}
		}

		public bool ApplyAction(ChatAction Action)
		{
			var r = Action.Apply(this);
			if (r && OnActionApplied != null) OnActionApplied(this, new ValuedEventArgs<ChatAction>(Action));
			return r;
		}

		public bool AddChatMessage(ChatMessage Message)
		{
			_Messages.Add(Message);
			return true;
		}
	}
}
