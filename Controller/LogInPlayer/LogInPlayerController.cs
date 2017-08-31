using System;

using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class LogInPlayerController
	{
		public EventHandler<ValuedEventArgs<PlayerContext>> OnLogIn;

		LogInPlayerScreen _Screen;

		public LogInPlayerController(LogInPlayerScreen Screen)
		{
			_Screen = Screen;
			_Screen.OnLogIn += HandleLogIn;
		}

		void HandleLogIn(object Sender, EventArgs E)
		{
			PlayerContext context = null;
			try
			{
				NetworkContext client = NetworkContext.CreateClient(_Screen.IpAddress, GameData.OnlinePort);
				try
				{
					context = client.MakeLoggedInPlayerContext(_Screen.Username, _Screen.Password);
				}
				catch
				{
					_Screen.SetError("Invalid Log In information.");
					client.Close();
				}
			}
			catch
			{
				_Screen.SetError("Could not connect to server.");
			}
			if (context != null && OnLogIn != null) OnLogIn(this, new ValuedEventArgs<PlayerContext>(context));
		}
	}
}
