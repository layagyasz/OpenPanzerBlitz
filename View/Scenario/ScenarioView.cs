using System;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public class ScenarioView : GuiContainer<GuiItem>
	{
		ScrollCollection<ClassedGuiItem> _ScenarioDisplay =
			new ScrollCollection<ClassedGuiItem>("scenario-main-display");
		SingleColumnTable _DetailDisplay = new SingleColumnTable("scenario-detail-display");

		public ScenarioView()
			: base("scenario-display")
		{
			_DetailDisplay.Position = new Vector2f(_ScenarioDisplay.Size.X + 16, 0);

			Add(_ScenarioDisplay);
			Add(_DetailDisplay);
		}

		public ScenarioView(Scenario Scenario, UnitConfigurationRenderer UnitRenderer, FactionRenderer FactionRenderer)
			: this()
		{
			SetScenario(Scenario, UnitRenderer, FactionRenderer);
		}

		public void SetScenario(
			Scenario Scenario, UnitConfigurationRenderer UnitRenderer, FactionRenderer FactionRenderer)
		{
			_ScenarioDisplay.Clear();
			foreach (ArmyConfiguration army in Scenario.ArmyConfigurations)
			{
				_ScenarioDisplay.Add(
					new Button("scenario-army-header")
					{
						DisplayedString = ObjectDescriber.Describe(army.Faction)
					});
				GuiContainer<GuiItem> factionMount = new GuiContainer<GuiItem>("scenario-faction-mount");
				Vector2f size = factionMount.Size - factionMount.LeftPadding * 2;
				FactionView faction =
					new FactionView(army.Faction, FactionRenderer, Math.Min(size.X, size.Y));
				faction.Position = .5f * (factionMount.Size - faction.Size) - factionMount.LeftPadding;
				factionMount.Add(faction);
				_ScenarioDisplay.Add(factionMount);
				foreach (DeploymentConfiguration deployment in army.DeploymentConfigurations)
					_ScenarioDisplay.Add(new DeploymentRow(deployment, army.Faction, UnitRenderer));
				foreach (ObjectiveSuccessTrigger trigger in army.VictoryCondition.Triggers)
					_ScenarioDisplay.Add(new VictoryConditionRow(trigger));
			}

			_DetailDisplay.Clear();
			AddDetail("Environment", ObjectDescriber.Describe(Scenario.Environment));
			AddDetail("Turns", Scenario.Turns.ToString());
			AddDetail(
				"Deploy Order", Scenario.DeploymentOrder.Select(i => ObjectDescriber.Describe(i.Faction)).ToArray());
			AddDetail("Turn Order", Scenario.TurnOrder.Select(i => ObjectDescriber.Describe(i.Faction)).ToArray());
		}

		void AddDetail(string Attribute, params string[] Values)
		{
			_DetailDisplay.Add(new Button("header-2") { DisplayedString = Attribute });
			foreach (string value in Values) _DetailDisplay.Add(new Button("regular") { DisplayedString = value });
		}
	}
}
