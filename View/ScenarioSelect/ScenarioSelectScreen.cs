using System;
using System.Collections.Generic;

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

		ValuedScrollCollection<SelectionOption<Scenario>, Scenario> _ScenarioSelect =
			new ValuedScrollCollection<SelectionOption<Scenario>, Scenario>("scenario-select");

		public ScenarioSelectScreen(Vector2f WindowSize, IEnumerable<Scenario> Scenarios)
			: base(WindowSize)
		{
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
			_ScenarioSelect.Position = .5f * (WindowSize - _ScenarioSelect.Size);
		}

		void HandleSelect(object Sender, ValuedEventArgs<SelectionOption<Scenario>> E)
		{
			if (OnScenarioSelected != null)
				OnScenarioSelected(this, new ValuedEventArgs<Scenario>(E.Value.Value));
		}

		public override void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);
			_ScenarioSelect.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public override void Draw(RenderTarget Target, Transform Transform)
		{
			base.Draw(Target, Transform);
			_ScenarioSelect.Draw(Target, Transform);
		}
	}
}
