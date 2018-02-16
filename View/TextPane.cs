using System;

using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public class TextPane : Pane
	{
		public EventHandler<ValuedEventArgs<string>> OnValueEntered;

		readonly SingleColumnTable _Display = new SingleColumnTable("text-pane-display");
		readonly TextInput _IPInput = new TextInput("text-pane-input");
		readonly Button _Error = new Button("text-pane-error");
		readonly Button _Button = new Button("small-button");

		public TextPane(string Title, string Subtitle, string ButtonString)
			: base("text-pane")
		{
			_Button.Position =
				new Vector2f(Size.X - _Button.Size.X - 32, Size.Y - _Button.Size.Y - 32);
			_Button.OnClick += HandleConnectButtonClick;
			_IPInput.OnSubmitted += HandleConnectButtonClick;

			_Display.Add(new Button("header-1") { DisplayedString = Title });
			_Display.Add(new Button("header-2") { DisplayedString = Subtitle });
			_Display.Add(_IPInput);

			_Button.DisplayedString = ButtonString;

			Add(_Display);
			Add(_Button);
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
