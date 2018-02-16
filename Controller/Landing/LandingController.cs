using System;

using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class LandingController
	{
		public EventHandler<ValuedEventArgs<MatchLobbyContext>> OnConnectionSetup;

		LandingScreen _LandingScreen;
		TextPane _ConnectionPane;

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
			_ConnectionPane = new TextPane("Remote Connection", "IP Address", "Connect");
			_ConnectionPane.OnValueEntered += HandleRemoteConnect;
			_LandingScreen.PaneLayer.Add(_ConnectionPane);
		}

		void HandleRemoteConnect(object Sender, ValuedEventArgs<string> E)
		{
			try
			{
				var client = NetworkContext.CreateClient(E.Value, GameData.OnlinePort);
				var lobby = client.MakePlayerContext(GameData.Player).MakeLobbyContext();
				if (lobby != null) OnConnectionSetup(this, new ValuedEventArgs<MatchLobbyContext>(lobby));
				else
				{
					_ConnectionPane.SetError("Failed to join lobby");
					client.Close();
				}
			}
			catch
			{
				_ConnectionPane.SetError("Failed to establish connection");
			}
		}
	}
}
