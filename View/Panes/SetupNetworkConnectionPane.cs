using System;

using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public class SetupNetworkConnectionPane : Pane
	{
		public EventHandler<ValuedEventArgs<string>> OnConnectionSetup;

		SingleColumnTable _Display = new SingleColumnTable("setup-network-connection-display");
		TextInput _IPInput = new TextInput("setup-network-connection-text-input");
		Button _ConnectButton = new Button("small-button") { DisplayedString = "Connect" };

		public SetupNetworkConnectionPane()
			: base("setup-network-connection-pane")
		{
			_ConnectButton.Position =
				new Vector2f(Size.X - _ConnectButton.Size.X - 32, Size.Y - _ConnectButton.Size.Y - 32);
			_ConnectButton.OnClick += HandleConnectButtonClick;

			_Display.Add(new Button("header-1") { DisplayedString = "Remote Connection" });
			_Display.Add(new Button("header-2") { DisplayedString = "IP Address" });
			_Display.Add(_IPInput);

			Add(_Display);
			Add(_ConnectButton);
		}

		void HandleConnectButtonClick(object Sender, EventArgs E)
		{
			if (OnConnectionSetup != null) OnConnectionSetup(this, new ValuedEventArgs<string>(_IPInput.Value));
		}
	}
}
