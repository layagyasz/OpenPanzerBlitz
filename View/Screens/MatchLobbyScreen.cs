using System;
using System.Collections.Generic;

using Cardamom.Interface;
using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class MatchLobbyScreen : ScreenBase
	{
		public EventHandler<ValuedEventArgs<Tuple<Player, ArmyConfiguration>>> OnArmyConfigurationSelected;
		public EventHandler<ValuedEventArgs<Tuple<Player, bool>>> OnPlayerReadyStateChanged;

		GuiContainer<Pod> _Pane = new GuiContainer<Pod>("match-lobby-pane");
		SingleColumnTable _Display = new SingleColumnTable("match-lobby-display");
		public readonly ChatView ChatView;

		bool _Dirty;
		bool _Host;
		MatchLobby _Lobby;
		Player _Player;

		public MatchLobbyScreen(Vector2f WindowSize, bool Host, MatchLobby Lobby, Chat Chat, Player Player)
			: base(WindowSize)
		{
			_Host = Host;
			_Lobby = Lobby;
			_Lobby.OnActionApplied += (sender, e) => _Dirty = true;
			_Player = Player;

			ChatView = new ChatView(
				Chat, "match-lobby-chat-display", "match-lobby-chat", "match-lobby-chat-message", "text-input");
			ChatView.Position = new Vector2f(_Display.Size.X + 16, 0);

			_Pane.Position = .5f * (WindowSize - _Pane.Size);
			_Pane.Add(_Display);
			_Pane.Add(ChatView);
			DisplayPlayers();
		}

		void DisplayPlayers()
		{
			_Display.Clear();
			_Display.Add(new Button("header-1") { DisplayedString = _Host ? "Host Match" : "Remote Match" });
			_Display.Add(new Button("header-2") { DisplayedString = "Player Setup" });
			foreach (Player p in _Lobby.Players)
			{
				MatchLobbyPlayerSection section = new MatchLobbyPlayerSection(p, _Lobby, p == _Player);
				section.OnArmyConfigurationSelected += HandleArmyConfigurationSelected;
				section.OnPlayerReadyStateChanged += HandlePlayerReadyStateChanged;
				_Display.Add(section);
			}
			_Dirty = false;
		}

		void HandleArmyConfigurationSelected(object Sender, ValuedEventArgs<Tuple<Player, ArmyConfiguration>> E)
		{
			if (OnArmyConfigurationSelected != null) OnArmyConfigurationSelected(this, E);
		}

		void HandlePlayerReadyStateChanged(object Sender, ValuedEventArgs<Tuple<Player, bool>> E)
		{
			if (OnPlayerReadyStateChanged != null) OnPlayerReadyStateChanged(this, E);
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			if (_Dirty) DisplayPlayers();
			_Pane.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			base.Draw(Target, Transform);
			_Pane.Draw(Target, Transform);
		}
	}
}
