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
			try
			{
				NetworkContext client = NetworkContext.CreateClient(_Screen.IpAddress, GameData.OnlinePort);
				PlayerContext context = client.MakeLoggedInPlayerContext(_Screen.Username, _Screen.Password);
				if (context == null)
				{
					_Screen.SetError("Invalid Log In information.");
					client.Close();
				}
				else if (OnLogIn != null) OnLogIn(this, new ValuedEventArgs<PlayerContext>(context));
			}
			catch
			{
				_Screen.SetError("Could not connect to server.");
			}
		}
	}
}
