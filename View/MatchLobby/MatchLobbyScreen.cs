using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class MatchLobbyScreen : ScreenBase
	{
		public EventHandler<ValuedEventArgs<Scenario>> OnScenarioSelected;
		public EventHandler<ValuedEventArgs<Tuple<Player, ArmyConfiguration>>> OnArmyConfigurationSelected;
		public EventHandler<ValuedEventArgs<Tuple<Player, bool>>> OnPlayerReadyStateChanged;
		public EventHandler<EventArgs> OnLaunched;
		public EventHandler<EventArgs> OnPulse;

		GuiContainer<Pod> _Pane = new GuiContainer<Pod>("match-lobby-pane");
		SingleColumnTable _Display = new SingleColumnTable("match-lobby-display");
		Select<Scenario> _ScenarioSelect = new Select<Scenario>("match-lobby-player-section-select");
		Button _LaunchButton = new Button("large-button") { DisplayedString = "Launch" };
		public readonly ChatView ChatView;

		bool _Dirty;
		bool _Host;
		MatchLobby _Lobby;
		Player _Player;
		List<Scenario> _Scenarios;

		public MatchLobbyScreen(Vector2f WindowSize, bool Host, MatchLobby Lobby, Chat Chat, Player Player, IEnumerable<Scenario> Scenarios)
			: base(WindowSize)
		{
			_Host = Host;
			_Lobby = Lobby;
			_Lobby.OnActionApplied += (sender, e) => _Dirty = true;
			_Player = Player;
			_Scenarios = Scenarios.ToList();

			ChatView = new ChatView(
				Chat, "match-lobby-chat-display", "match-lobby-chat", "match-lobby-chat-message", "text-input");
			ChatView.Position = new Vector2f(_Display.Size.X + 16, 0);

			_LaunchButton.Position = new Vector2f(0, _Pane.Size.Y - _LaunchButton.Size.Y - 32);
			_LaunchButton.Enabled = Host;
			_LaunchButton.OnClick += HandleLaunched;

			_ScenarioSelect.Enabled = Host;
			_ScenarioSelect.OnChange += HandleScenarioSelected;

			_Pane.Position = .5f * (WindowSize - _Pane.Size);
			_Pane.Add(_Display);
			_Pane.Add(_LaunchButton);
			_Pane.Add(ChatView);
			DisplayPlayers();
		}

		void DisplayPlayers()
		{
			_Display.Clear();
			_Display.Add(new Button("header-1") { DisplayedString = _Host ? "Host Match" : "Remote Match" });
			_Display.Add(new Button("header-2") { DisplayedString = "Scenario" });
			_Display.Add(new GuiContainer<Pod>("regular") { _ScenarioSelect });
			_Display.Add(new Button("header-2") { DisplayedString = "Player Setup" });

			if (_Host)
			{
				if (_ScenarioSelect.Count() == 0)
				{
					foreach (Scenario s in _Scenarios)
					{
						_ScenarioSelect.Add(new SelectionOption<Scenario>("match-lobby-player-section-select-option")
						{
							DisplayedString = s.Name,
							Value = s
						});
					}
				}
			}
			else
			{
				_ScenarioSelect.Clear();
				_ScenarioSelect.Add(new SelectionOption<Scenario>("match-lobby-player-section-select-option")
				{
					DisplayedString = _Lobby.Scenario.Name,
					Value = _Lobby.Scenario,
				});
			}
			_ScenarioSelect.Enabled = _Host;

			foreach (Player p in _Lobby.Players)
			{
				MatchLobbyPlayerSection section = new MatchLobbyPlayerSection(p, _Lobby, p == _Player);
				section.OnArmyConfigurationSelected += HandleArmyConfigurationSelected;
				section.OnPlayerReadyStateChanged += HandlePlayerReadyStateChanged;
				_Display.Add(section);
			}
			_Dirty = false;
		}

		void HandleScenarioSelected(object Sender, ValuedEventArgs<StandardItem<Scenario>> E)
		{
			if (OnScenarioSelected != null) OnScenarioSelected(this, new ValuedEventArgs<Scenario>(E.Value.Value));
		}

		void HandleArmyConfigurationSelected(object Sender, ValuedEventArgs<Tuple<Player, ArmyConfiguration>> E)
		{
			if (OnArmyConfigurationSelected != null) OnArmyConfigurationSelected(this, E);
		}

		void HandlePlayerReadyStateChanged(object Sender, ValuedEventArgs<Tuple<Player, bool>> E)
		{
			if (OnPlayerReadyStateChanged != null) OnPlayerReadyStateChanged(this, E);
		}

		void HandleLaunched(object Sender, EventArgs E)
		{
			if (OnLaunched != null) OnLaunched(this, EventArgs.Empty);
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			if (OnPulse != null) OnPulse(this, EventArgs.Empty);
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
