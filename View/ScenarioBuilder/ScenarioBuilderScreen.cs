﻿using System;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public class ScenarioBuilderScreen : ScreenBase
	{
		GuiContainer<Pod> _Pane = new GuiContainer<Pod>("scenario-builder-pane");
		SingleColumnTable _Display = new SingleColumnTable("scenario-builder-display");

		Select<uint> _YearSelect = new Select<uint>("scenario-builder-parameters-section-select");
		Select<Environment> _EnvironmentSelect = new Select<Environment>("scenario-builder-parameters-section-select");
		Select<Front> _FrontSelect = new Select<Front>("scenario-builder-parameters-section-select");

		public ScenarioBuilderScreen(Vector2f WindowSize)
			: base(WindowSize, true)
		{
			_Display.Add(new Button("header-1") { DisplayedString = "Custom Scenario" });

			MakeSection("Year", _YearSelect, _Display);
			for (uint i = 1939; i < 1946; ++i)
				_YearSelect.Add(
					new SelectionOption<uint>(
						"scenario-builder-parameters-section-select-option")
					{
						DisplayedString = i.ToString(),
						Value = i
					});

			MakeSection("Environment", _EnvironmentSelect, _Display);
			foreach (Environment environment in GameData.Environments.Values)
				_EnvironmentSelect.Add(
					new SelectionOption<Environment>("scenario-builder-parameters-section-select-option")
					{
						DisplayedString = ObjectDescriber.Describe(environment),
						Value = environment
					});

			MakeSection("Front", _FrontSelect, _Display);
			foreach (Front front in Enum.GetValues(typeof(Front)).Cast<Front>().Skip(1))
				_FrontSelect.Add(
					new SelectionOption<Front>("scenario-builder-parameters-section-select-option")
					{
						DisplayedString = ObjectDescriber.Describe(front),
						Value = front
					});

			_Pane.Position = .5f * (WindowSize - _Pane.Size);

			_Pane.Add(_Display);
			_Items.Add(_Pane);
		}

		void MakeSection(string SectionName, GuiItem Input, SingleColumnTable Display)
		{
			Button header = new Button("header-2") { DisplayedString = SectionName };
			GuiContainer<Pod> container = new GuiContainer<Pod>("scenario-builder-parameters-section");

			container.Add(Input);

			Display.Add(header);
			Display.Add(container);
		}
	}
}
