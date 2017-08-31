using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	// For now this is basically the same as LogInPlayerScreen.  That will probably change in the future as more player
	// information is added.
	public class RegisterPlayerScreen : ScreenBase
	{
		public EventHandler<EventArgs> OnRegister;
		public EventHandler<EventArgs> OnLogIn;

		GuiContainer<Pod> _Pane = new GuiContainer<Pod>("join-server-pane");
		SingleColumnTable _Display = new SingleColumnTable("join-server-display");
		TextInput _ServerInput = new TextInput("join-server-text-input");
		TextInput _UsernameInput = new TextInput("join-server-text-input");
		TextInput _PasswordInput = new TextInput("join-server-text-input");
		Button _Error = new Button("join-server-error");
		Button _RegisterButton = new Button("join-server-button") { DisplayedString = "Register" };
		Button _LogInButton = new Button("join-server-button") { DisplayedString = "Back To Log In" };

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

		public RegisterPlayerScreen(Vector2f WindowSize)
			: base(WindowSize)
		{
			_Display.Add(new Button("join-server-header-1") { DisplayedString = "Register" });
			_Display.Add(new Button("join-server-header-2") { DisplayedString = "Server IP" });
			_Display.Add(_ServerInput);
			_Display.Add(new Button("join-server-header-2") { DisplayedString = "Username" });
			_Display.Add(_UsernameInput);
			_Display.Add(new Button("join-server-header-2") { DisplayedString = "Password" });
			_Display.Add(_PasswordInput);

			_LogInButton.Position = new Vector2f(0, _Pane.Size.Y - _LogInButton.Size.Y - 32);
			_LogInButton.OnClick += HandleLogIn;

			_RegisterButton.Position = new Vector2f(0, _LogInButton.Position.Y - _RegisterButton.Size.Y - 4);
			_RegisterButton.OnClick += HandleRegister;

			_Pane.Position = .5f * (WindowSize - _Pane.Size);
			_Pane.Add(_Display);
			_Pane.Add(_RegisterButton);
			_Pane.Add(_LogInButton);
		}

		public void SetError(string Message)
		{
			_Error.DisplayedString = Message;
			_Display.Remove(_Error);
			_Display.Add(_Error);
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			_Pane.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			base.Draw(Target, Transform);
			_Pane.Draw(Target, Transform);
		}

		void HandleRegister(object Sender, EventArgs E)
		{
			if (OnRegister != null) OnRegister(this, E);
		}

		void HandleLogIn(object Sender, EventArgs E)
		{
			if (OnLogIn != null) OnLogIn(this, E);
		}
	}
}
