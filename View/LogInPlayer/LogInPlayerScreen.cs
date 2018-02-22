using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public class LogInPlayerScreen : ScreenBase
	{
		public EventHandler<EventArgs> OnLogIn;
		public EventHandler<EventArgs> OnRegister;

		GuiContainer<Pod> _Pane = new GuiContainer<Pod>("join-server-pane");
		SingleColumnTable _Display = new SingleColumnTable("join-server-display");
		TextInput _ServerInput = new TextInput("join-server-text-input");
		TextInput _UsernameInput = new TextInput("join-server-text-input");
		TextInput _PasswordInput = new TextInput("join-server-text-input");
		Button _Error = new Button("join-server-error");
		Button _LogInButton = new Button("join-server-button") { DisplayedString = "Log In" };
		Button _RegisterButton = new Button("join-server-button") { DisplayedString = "Go to Registration" };

		public string IpAddress
		{
			get
			{
				return _ServerInput.Value;
			}
		}

		public string Username
		{
			get
			{
				return _UsernameInput.Value;
			}
		}

		public string Password
		{
			get
			{
				return _PasswordInput.Value;
			}
		}

		public LogInPlayerScreen(Vector2f WindowSize)
			: base(WindowSize)
		{
			_Display.Add(new Button("join-server-header-1") { DisplayedString = "Log In" });
			_Display.Add(new Button("join-server-header-2") { DisplayedString = "Server IP" });
			_Display.Add(_ServerInput);
			_Display.Add(new Button("join-server-header-2") { DisplayedString = "Username" });
			_Display.Add(_UsernameInput);
			_Display.Add(new Button("join-server-header-2") { DisplayedString = "Password" });
			_Display.Add(_PasswordInput);

			_RegisterButton.Position = new Vector2f(0, _Pane.Size.Y - _RegisterButton.Size.Y - 32);
			_RegisterButton.OnClick += HandleRegister;

			_LogInButton.Position = new Vector2f(0, _RegisterButton.Position.Y - _LogInButton.Size.Y - 4);
			_LogInButton.OnClick += HandleLogIn;

			_Pane.Position = .5f * (WindowSize - _Pane.Size);
			_Pane.Add(_Display);
			_Pane.Add(_LogInButton);
			_Pane.Add(_RegisterButton);

			_Items.Add(_Pane);
		}

		public void SetError(string Message)
		{
			_Error.DisplayedString = Message;
			_Display.Remove(_Error);
			_Display.Add(_Error);
		}

		void HandleLogIn(object Sender, EventArgs E)
		{
			if (OnLogIn != null) OnLogIn(this, E);
		}

		void HandleRegister(object Sender, EventArgs E)
		{
			if (OnRegister != null) OnRegister(this, E);
		}
	}
}
