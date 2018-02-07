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
	public class ScenarioSelectScreen : ScreenBase
	{
		public EventHandler<ValuedEventArgs<Scenario>> OnScenarioSelected;

		UnitConfigurationRenderer _Renderer;

		GuiContainer<GuiItem> _Display = new GuiContainer<GuiItem>("scenario-select-display");

		ValuedScrollCollection<SelectionOption<Scenario>, Scenario> _ScenarioSelect =
			new ValuedScrollCollection<SelectionOption<Scenario>, Scenario>("scenario-select");
		ScrollCollection<ClassedGuiItem> _ScenarioDisplay =
			new ScrollCollection<ClassedGuiItem>("scenario-select-scenario-display");

		public ScenarioSelectScreen(Vector2f WindowSize, IEnumerable<Scenario> Scenarios)
			: base(WindowSize)
		{
			_Renderer = new UnitConfigurationRenderer(
				Scenarios.SelectMany(i => i.UnitConfigurations).Distinct(),
				GameData.UnitRenderDetails,
				128,
				1024,
				new Font("Compacta Std Regular.otf"));
			foreach (Scenario s in Scenarios)
			{
				SelectionOption<Scenario> option = new SelectionOption<Scenario>("scenario-selection-option")
				{
					DisplayedString = s.Name,
					Value = s
				};
				_ScenarioSelect.Add(option);
			}
			_ScenarioSelect.OnChange += HandleSelect;
			_ScenarioDisplay.Position = new Vector2f(_ScenarioSelect.Size.X + 16, 0);

			_Display.Position = .5f * (WindowSize - _Display.Size);

			_Display.Add(_ScenarioSelect);
			_Display.Add(_ScenarioDisplay);
			_Items.Add(_Display);
		}

		void SetScenario(Scenario Scenario)
		{
			_ScenarioDisplay.Clear();
			foreach (ArmyConfiguration army in Scenario.ArmyConfigurations)
			{
				_ScenarioDisplay.Add(
					new Button("scenario-select-army-header")
					{
						DisplayedString = ObjectDescriber.Describe(army.Faction)
					});
				foreach (DeploymentConfiguration deployment in army.DeploymentConfigurations)
					_ScenarioDisplay.Add(new DeploymentRow(deployment, army.Faction, _Renderer));
				foreach (ObjectiveSuccessTrigger trigger in army.VictoryCondition.Triggers)
					_ScenarioDisplay.Add(new VictoryConditionRow(trigger));
			}
		}

		void HandleSelect(object Sender, ValuedEventArgs<SelectionOption<Scenario>> E)
		{
			SetScenario(E.Value.Value);
		}
	}
}
