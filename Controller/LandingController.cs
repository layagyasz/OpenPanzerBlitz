using System;

using Cardamom.Network;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class LandingController
	{
		public EventHandler<ValuedEventArgs<LobbyContext>> OnConnectionSetup;

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

		void HandleRemoteConnect(object Sender, ValuedEventArgs<string> E)
		{
			if (OnConnectionSetup != null)
			{
				TCPClient client = new TCPClient(E.Value, 1000);
				client.MessageAdapter = new NonGameMessageSerializer();
				LobbyRPCHandler handler = new LobbyRPCHandler();
				client.RPCHandler = handler;
				client.Start();

				client.Call(new ApplyLobbyActionRequest(new AddPlayerAction(GameData.Player))).Get();
				MatchLobby lobby = ((GetLobbyResponse)client.Call(
					new GetLobbyRequest()).Get()).Lobby;
				handler.SetLobby(lobby);

				OnConnectionSetup(this, new ValuedEventArgs<LobbyContext>(new LobbyContext(client, lobby)));
			}
		}
	}
}
