using System;

using Cardamom.Interface;
using Cardamom.Network;

namespace PanzerBlitz
{
	public class LandingController
	{
		public EventHandler<ValueChangedEventArgs<TCPClient>> OnConnectionSetup;

		LandingScreen _LandingScreen;
		SetupNetworkConnectionPane _ConnectionPane;

		public LandingController(LandingScreen LandingScreen)
		{
			_LandingScreen = LandingScreen;
			_LandingScreen.JoinRemoteMatchButton.OnClick += HandleRemoteConnectionSetup;
		}

		void Clear()
		{
			_LandingScreen.PaneLayer.Clear();
		}

		void HandleRemoteConnectionSetup(object Sender, EventArgs E)
		{
			_ConnectionPane = new SetupNetworkConnectionPane();
			_ConnectionPane.OnConnectionSetup += HandleRemoteConnect;
			_LandingScreen.PaneLayer.Add(_ConnectionPane);
		}

		void HandleRemoteConnect(object Sender, ValueChangedEventArgs<string> E)
		{
			if (OnConnectionSetup != null)
				OnConnectionSetup(this, new ValueChangedEventArgs<TCPClient>(new TCPClient(E.Value, 1000)));
		}
	}
}
