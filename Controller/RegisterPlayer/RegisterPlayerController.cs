﻿using System;

using Cardamom.Network;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class RegisterPlayerController
	{
		public EventHandler<ValuedEventArgs<PlayerContext>> OnRegister;

		readonly RegisterPlayerScreen _Screen;

		public RegisterPlayerController(RegisterPlayerScreen Screen)
		{
			_Screen = Screen;
			_Screen.OnRegister += HandleRegister;
		}

		void HandleRegister(object Sender, EventArgs E)
		{
			try
			{
				var client = NetworkContext.CreateClient(_Screen.IpAddress, GameData.OnlinePort);
				client.Client.MessageAdapter = new NonMatchMessageSerializer();
				client.Client.RPCHandler = new RPCHandler();
				Player p = client.Client.Call(
					new RegisterPlayerRequest(_Screen.Username, _Screen.Password)).Get<LogInPlayerResponse>().Player;
				if (p == null)
				{
					_Screen.SetError("Unable to Register.");
					client.Close();
				}
				else if (OnRegister != null)
					OnRegister(this, new ValuedEventArgs<PlayerContext>(client.MakePlayerContext(p)));
			}
			catch
			{
				_Screen.SetError("Could not connect to server.");
			}
		}
	}
}
