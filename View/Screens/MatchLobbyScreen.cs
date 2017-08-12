using System;
using System.Collections.Generic;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class MatchLobbyScreen : ScreenBase
	{
		GuiContainer<Pod> _Pane = new GuiContainer<Pod>("match-lobby-pane");
		ScrollCollection<object> _Display = new ScrollCollection<object>("match-lobby-display");

		bool _Host;
		MatchLobby _Lobby;

		public MatchLobbyScreen(Vector2f WindowSize, bool Host, MatchLobby Lobby)
			: base(WindowSize)
		{
			_Host = Host;
			_Lobby = Lobby;
			_Lobby.OnActionApplied += (sender, e) => DisplayPlayers();

			_Pane.Position = .5f * (WindowSize - _Pane.Size);
			_Pane.Add(_Display);
			DisplayPlayers();
		}

		void DisplayPlayers()
		{
			_Display.Clear();
			_Display.Add(new Button("header-1") { DisplayedString = _Host ? "Host Match" : "Remote Match" });
			foreach (KeyValuePair<Player, ArmyConfiguration> p in _Lobby.PlayerConfiguration)
				_Display.Add(new Button("regular") { DisplayedString = p.Key.Name });
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
	}
}
