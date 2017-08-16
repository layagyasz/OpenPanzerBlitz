using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public class MatchLobbyPlayerSection : GuiContainer<Pod>
	{
		public EventHandler<ValuedEventArgs<Tuple<Player, ArmyConfiguration>>> OnArmyConfigurationSelected;
		public EventHandler<ValuedEventArgs<Tuple<Player, bool>>> OnPlayerReadyStateChanged;

		public readonly Player Player;

		public MatchLobbyPlayerSection(Player Player, MatchLobby Lobby, bool Enabled)
			: base("match-lobby-player-section")
		{
			this.Player = Player;

			Button header = new Button("match-lobby-player-section-header") { DisplayedString = Player.Name };
			Select<ArmyConfiguration> select = new Select<ArmyConfiguration>("match-lobby-player-section-select");
			Checkbox ready = new Checkbox("match-lobby-player-section-checkbox");

			select.Add(new SelectionOption<ArmyConfiguration>("match-lobby-player-section-select-option")
			{
				DisplayedString = "Spectator",
				Value = null
			});
			foreach (ArmyConfiguration a in Lobby.Scenario.ArmyConfigurations)
			{
				select.Add(new SelectionOption<ArmyConfiguration>("match-lobby-player-section-select-option")
				{
					DisplayedString = a.Faction.Name,
					Value = a
				});
			}
			if (Enabled) select.OnChange += HandleArmyConfigurationSelected;
			select.Position = new Vector2f(0, header.Size.Y + 6);
			select.Enabled = Enabled;
			select.SetValue(i => i.Value == Lobby.GetPlayerArmy(Player));

			if (Enabled) ready.OnChange += HandlePlayerReadyStateChanged;
			ready.Position = new Vector2f(Size.X - ready.Size.X - 16, 0);
			ready.Value = Lobby.GetPlayerReady(Player);
			ready.Enabled = Enabled;

			Add(header);
			Add(select);
			Add(ready);
		}

		void HandleArmyConfigurationSelected(object Sender, ValuedEventArgs<StandardItem<ArmyConfiguration>> E)
		{
			if (OnArmyConfigurationSelected != null)
				OnArmyConfigurationSelected(
					this,
					new ValuedEventArgs<Tuple<Player, ArmyConfiguration>>(
						new Tuple<Player, ArmyConfiguration>(Player, E.Value.Value)));
		}

		void HandlePlayerReadyStateChanged(object Sender, ValuedEventArgs<bool> E)
		{
			if (OnPlayerReadyStateChanged != null)
				OnPlayerReadyStateChanged(
					this, new ValuedEventArgs<Tuple<Player, bool>>(new Tuple<Player, bool>(Player, E.Value)));
		}
	}
}
