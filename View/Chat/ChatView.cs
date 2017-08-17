using System;
using System.Collections.Generic;

using Cardamom.Interface;
using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class ChatView : SingleColumnTable
	{
		public EventHandler<ValuedEventArgs<string>> OnTextSubmitted;

		Chat _Chat;
		ScrollCollection<string> _Display;
		TextInput _TextInput;
		string _MessageClassName;

		List<ChatMessage> _IncomingMessages = new List<ChatMessage>();

		public ChatView(
			Chat Chat,
			string ContainerClassName,
			string PanelClassName,
			string MessageClassName,
			string TextInputClassName)
			: base(ContainerClassName)
		{
			_Display = new ScrollCollection<string>(PanelClassName);
			_TextInput = new TextInput(TextInputClassName);
			_TextInput.OnSubmitted += HandleTextSubmitted;
			_MessageClassName = MessageClassName;

			_Chat = Chat;
			_Chat.OnActionApplied += HandleChatAction;
			foreach (ChatMessage m in _Chat.Messages) AddChatMessage(m);

			Add(_Display);
			Add(_TextInput);
		}

		void AddChatMessage(ChatMessage Message)
		{
			_Display.Add(new TextBox(_MessageClassName)
			{
				DisplayedString = MakeMessageString(Message)
			});
		}

		void HandleChatAction(object Sender, ValuedEventArgs<ChatAction> E)
		{
			if (E.Value is AddChatMessageAction)
			{
				lock (_IncomingMessages)
				{
					_IncomingMessages.Add(((AddChatMessageAction)E.Value).Message);
				}
			}
		}

		void HandleTextSubmitted(object Sender, EventArgs E)
		{
			if (OnTextSubmitted != null)
			{
				OnTextSubmitted(null, new ValuedEventArgs<string>(_TextInput.Value));
				_TextInput.Value = "";
			}
		}

		string MakeMessageString(ChatMessage Message)
		{
			return string.Format(
				"{0} {1}: {2}", Message.Player.Name, Message.Time.ToShortTimeString(), Message.Message);
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);
			lock (_IncomingMessages)
			{
				foreach (ChatMessage m in _IncomingMessages) AddChatMessage(m);
				_IncomingMessages.Clear();
			}
		}
	}
}
