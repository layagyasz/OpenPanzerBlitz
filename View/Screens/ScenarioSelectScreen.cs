using System;
using System.Collections.Generic;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class ScenarioSelectScreen : Pod
	{
		static Color ORANGE = new Color(172, 107, 26);

		public EventHandler<ValueChangedEventArgs<Scenario>> OnScenarioSelected;

		Vertex[] _Backdrop;
		ScrollCollection<Scenario> _ScenarioSelect = new ScrollCollection<Scenario>("scenario-select");

		public ScenarioSelectScreen(Vector2f WindowSize, IEnumerable<Scenario> Scenarios)
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

			_Backdrop = new Vertex[]
			{
				new Vertex(new Vector2f(0, 0), ORANGE),
				new Vertex(new Vector2f(WindowSize.X, 0), ORANGE),
				new Vertex(new Vector2f(WindowSize.X, .33f * WindowSize.Y), ORANGE),
				new Vertex(new Vector2f(0, .33f * WindowSize.Y), ORANGE),

				new Vertex(new Vector2f(0, .33f * WindowSize.Y), Color.Black),
				new Vertex(new Vector2f(WindowSize.X, .33f * WindowSize.Y), Color.Black),
				new Vertex(WindowSize, Color.Black),
				new Vertex(new Vector2f(0, WindowSize.Y), Color.Black)
			};
		}

		void HandleSelect(object Sender, ValueChangedEventArgs<ClassedGuiInput<Scenario>> E)
		{
			if (OnScenarioSelected != null)
				OnScenarioSelected(this, new ValueChangedEventArgs<Scenario>(E.Value.Value));
		}

		public void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			_ScenarioSelect.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public void Draw(RenderTarget Target, Transform Transform)
		{
			Target.Draw(_Backdrop, PrimitiveType.Quads);
			_ScenarioSelect.Draw(Target, Transform);
		}
	}
}
