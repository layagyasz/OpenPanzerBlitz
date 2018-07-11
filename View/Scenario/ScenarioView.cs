using System;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public class ScenarioView : GuiContainer<GuiItem>
	{
		readonly ScrollCollection<ClassedGuiItem> _ScenarioDisplay =
			new ScrollCollection<ClassedGuiItem>("scenario-main-display");
		readonly SingleColumnTable _DetailDisplay = new SingleColumnTable("scenario-detail-display");

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
				var factionMount = new GuiContainer<GuiItem>("scenario-faction-mount");
				Vector2f size = factionMount.Size - factionMount.LeftPadding * 2;
				var faction = new FactionView(army.Faction, FactionRenderer, Math.Min(size.X, size.Y));
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
			AddDetail("Turns", Scenario.TurnConfiguration.Turns.ToString());
			AddSequence("Deploy Order", Scenario.TurnConfiguration.DeploymentOrder, Scenario.ArmyConfigurations.ToArray());
			AddSequence("Turn Order", Scenario.TurnConfiguration.TurnOrder, Scenario.ArmyConfigurations.ToArray());
			AddDetail("Strength", Scenario.ArmyConfigurations.Select(i => DescribeStrength(i)).ToArray());
		}

		void AddDetail(string Attribute, params string[] Values)
		{
			_DetailDisplay.Add(new Button("header-2") { DisplayedString = Attribute });
			foreach (string value in Values) _DetailDisplay.Add(new Button("regular") { DisplayedString = value });
		}

		void AddSequence<T>(string SequenceName, Sequence Sequence, T[] Source)
		{
			if (Sequence is StaticSequence)
				AddDetail(
					SequenceName, Sequence.Get(null, 1).Select(i => ObjectDescriber.Describe(Source[i])).ToArray());
			else if (Sequence is RandomSequence)
				AddDetail(SequenceName, "Random");
		}

		string DescribeStrength(ArmyConfiguration Configuration)
		{
			var unitList = Configuration.BuildUnitConfigurationList().ToList();
			return string.Format(
				"{0} - {1} Points / {2} Units",
				ObjectDescriber.Describe(Configuration.Faction),
				unitList.Sum(i => i.GetPointValue(Configuration.Faction.HalfPriceTrucks)),
				unitList.Count);
		}
	}
}
