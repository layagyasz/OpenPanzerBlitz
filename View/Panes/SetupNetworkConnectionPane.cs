using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public class SetupNetworkConnectionPane : Pane
	{
		public EventHandler<ValueChangedEventArgs<string>> OnConnectionSetup;

		TextInput _IPInput = new TextInput("setup-network-connection-text-input");
		Button _ConnectButton = new Button("small-button") { DisplayedString = "Connect" };

		public SetupNetworkConnectionPane()
			: base("setup-network-connection-pane")
		{
			_ConnectButton.Position = new Vector2f(Size.X - _ConnectButton.Size.X - 32, _IPInput.Size.Y + 6);
			_ConnectButton.OnClick += HandleConnectButtonClick;

			Add(_IPInput);
			Add(_ConnectButton);
		}

		void HandleConnectButtonClick(object Sender, EventArgs E)
		{
			if (OnConnectionSetup != null) OnConnectionSetup(this, new ValueChangedEventArgs<string>(_IPInput.Value));
		}
	}
}
