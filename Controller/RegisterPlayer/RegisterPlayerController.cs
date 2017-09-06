using System;

using Cardamom.Network;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class RegisterPlayerController
	{
		public EventHandler<ValuedEventArgs<PlayerContext>> OnRegister;

		RegisterPlayerScreen _Screen;

		public RegisterPlayerController(RegisterPlayerScreen Screen)
		{
			_Screen = Screen;
			_Screen.OnRegister += HandleRegister;
		}

		void HandleRegister(object Sender, EventArgs E)
		{
			try
			{
				NetworkContext client = NetworkContext.CreateClient(_Screen.IpAddress, GameData.OnlinePort);
				client.Client.MessageAdapter = new NonMatchMessageSerializer();
				client.Client.RPCHandler = new RPCHandler();
				Player p = ((LogInPlayerResponse)client.Client.Call(
					new RegisterPlayerRequest(_Screen.Username, _Screen.Password)).Get()).Player;
				if (p == null)
				{
					_Screen.SetError("Unable to Register.");
					client.Close();
				}
				if (OnRegister != null)
					OnRegister(this, new ValuedEventArgs<PlayerContext>(client.MakePlayerContext(p)));
			}
			catch
			{
				_Screen.SetError("Could not connect to server.");
			}
		}
	}
}
