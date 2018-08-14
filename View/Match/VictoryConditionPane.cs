using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public class VictoryConditionPane : Pane
	{
		public EventHandler<EventArgs> OnClose;

		Button _CloseButton = new Button("victory-condition-close-button") { DisplayedString = "X" };

		readonly ScrollCollection<ClassedGuiItem> _VictoryConditionDisplay =
					new ScrollCollection<ClassedGuiItem>("victory-condition-display");

		public VictoryConditionPane(Match Match, FactionRenderer FactionRenderer)
			: base("victory-condition-pane")
		{
			_CloseButton.Position = new Vector2f(Size.X - _CloseButton.Size.X - LeftPadding.X * 2, 0);
			_CloseButton.OnClick += HandleClose;

			_VictoryConditionDisplay.Position = new Vector2f(0, _CloseButton.Size.Y + 24);
			foreach (ArmyConfiguration army in Match.Scenario.ArmyConfigurations)
			{
				_VictoryConditionDisplay.Add(
					new Button("scenario-army-header")
					{
						DisplayedString = ObjectDescriber.Describe(army.Faction)
					});
				var factionMount = new GuiContainer<GuiItem>("scenario-faction-mount");
				Vector2f size = factionMount.Size - factionMount.LeftPadding * 2;
				var faction = new FactionView(army.Faction, FactionRenderer, Math.Min(size.X, size.Y));
				faction.Position = .5f * (factionMount.Size - faction.Size) - factionMount.LeftPadding;
				factionMount.Add(faction);
				_VictoryConditionDisplay.Add(factionMount);
				foreach (ObjectiveSuccessTrigger trigger in army.VictoryCondition.Triggers)
					_VictoryConditionDisplay.Add(new VictoryConditionRow(trigger));
			}

			Add(_CloseButton);
			Add(_VictoryConditionDisplay);
		}

		void HandleClose(object Sender, EventArgs E)
		{
			OnClose?.Invoke(this, EventArgs.Empty);
		}
	}
}
