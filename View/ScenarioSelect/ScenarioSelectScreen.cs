using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public class ScenarioSelectScreen : ScreenBase
	{
		public EventHandler<ValuedEventArgs<Scenario>> OnScenarioSelected;

		UnitConfigurationRenderer _Renderer;
		FactionRenderer _FactionRenderer;

		GuiContainer<GuiItem> _Display = new GuiContainer<GuiItem>("scenario-select-display");

		readonly ValuedScrollCollection<SelectionOption<Scenario>, Scenario> _ScenarioSelect =
			new ValuedScrollCollection<SelectionOption<Scenario>, Scenario>("scenario-select");
		readonly ScenarioView _ScenarioView = new ScenarioView();
		readonly Button _StartButton = new Button("large-button") { DisplayedString = "Play" };

		public ScenarioSelectScreen(Vector2f WindowSize, IEnumerable<Scenario> Scenarios)
			: base(WindowSize)
		{
			_Renderer =
				new UnitConfigurationRenderer(
					Scenarios.SelectMany(i => i.UnitConfigurations).Distinct(),
					GameData.UnitRenderDetails,
					128,
					1024,
					ClassLibrary.Instance.GetFont("compacta"));
			_FactionRenderer =
				new FactionRenderer(
					Scenarios.SelectMany(i => i.ArmyConfigurations.Select(j => j.Faction)).Distinct(),
					GameData.FactionRenderDetails,
					512,
					1024);
			foreach (Scenario s in Scenarios)
			{
				var option = new SelectionOption<Scenario>("scenario-selection-option")
				{
					DisplayedString = s.Name,
					Value = s
				};
				_ScenarioSelect.Add(option);
			}
			_ScenarioSelect.OnChange += HandleSelect;
			_ScenarioView.Position = new Vector2f(_ScenarioSelect.Size.X + 16, 0);

			_StartButton.OnClick += HandleStart;
			_StartButton.Position = new Vector2f(0, _Display.Size.Y - _StartButton.Size.Y - 32);

			_Display.Position = .5f * (WindowSize - _Display.Size);

			_Display.Add(_ScenarioSelect);
			_Display.Add(_ScenarioView);
			_Display.Add(_StartButton);
			_Items.Add(_Display);
		}

		void SetScenario(Scenario Scenario)
		{
			_ScenarioView.SetScenario(Scenario, _Renderer, _FactionRenderer);
		}

		void HandleSelect(object Sender, ValuedEventArgs<SelectionOption<Scenario>> E)
		{
			SetScenario(E.Value.Value);
		}

		void HandleStart(object Sender, EventArgs E)
		{
			if (OnScenarioSelected != null && _ScenarioSelect.Value != null)
				OnScenarioSelected(this, new ValuedEventArgs<Scenario>(_ScenarioSelect.Value.Value));
		}
	}
}
