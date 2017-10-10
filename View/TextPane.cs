using System;

using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public class TextPane : Pane
	{
		public EventHandler<ValuedEventArgs<string>> OnValueEntered;

		SingleColumnTable _Display = new SingleColumnTable("text-pane-display");
		TextInput _IPInput = new TextInput("text-pane-input");
		Button _Error = new Button("text-pane-error");
		Button _ConnectButton = new Button("small-button") { DisplayedString = "Connect" };

		public TextPane(string Title, string Subtitle)
			: base("text-pane")
		{
			_ConnectButton.Position =
				new Vector2f(Size.X - _ConnectButton.Size.X - 32, Size.Y - _ConnectButton.Size.Y - 32);
			_ConnectButton.OnClick += HandleConnectButtonClick;
			_IPInput.OnSubmitted += HandleConnectButtonClick;

			_Display.Add(new Button("header-1") { DisplayedString = Title });
			_Display.Add(new Button("header-2") { DisplayedString = Subtitle });
			_Display.Add(_IPInput);

			Add(_Display);
			Add(_ConnectButton);
		}

		public void SetError(string Message)
		{
			_Error.DisplayedString = Message;
			_Display.Remove(_Error);
			_Display.Add(_Error);
		}

		void HandleConnectButtonClick(object Sender, EventArgs E)
		{
			if (OnValueEntered != null) OnValueEntered(this, new ValuedEventArgs<string>(_IPInput.Value));
		}
	}
}
