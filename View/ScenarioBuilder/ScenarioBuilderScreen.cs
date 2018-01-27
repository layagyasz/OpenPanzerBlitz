using System;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Window;

using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class ScenarioBuilderScreen : ScreenBase
	{
		public EventHandler<ValuedEventArgs<ScenarioParameters>> OnParametersChanged;
		public EventHandler OnArmyAdded;
		public EventHandler<ValuedEventArgs<Tuple<ArmyBuilder, ArmyParameters>>> OnArmyParametersChanged;
		public EventHandler<ValuedEventArgs<ArmyBuilder>> OnArmyRemoved;
		public EventHandler OnFinished;

		public readonly ScenarioBuilder ScenarioBuilder;

		GuiContainer<Pod> _Pane = new GuiContainer<Pod>("scenario-builder-pane");
		SingleColumnTable _Display = new SingleColumnTable("scenario-builder-display");
		Button _Error = new Button("footer-error");

		Select<uint> _YearSelect = new Select<uint>("scenario-builder-parameters-section-select");
		Select<Environment> _EnvironmentSelect = new Select<Environment>("scenario-builder-parameters-section-select");
		Select<Front> _FrontSelect = new Select<Front>("scenario-builder-parameters-section-select");

		Table _ArmiesTable = new Table("scenario-builder-army-section-table", true);

		public ScenarioBuilderScreen(Vector2f WindowSize, ScenarioBuilder ScenarioBuilder)
			: base(WindowSize, true)
		{
			this.ScenarioBuilder = ScenarioBuilder;

			_Display.Add(new Button("header-1") { DisplayedString = "Custom Scenario" });

			MakeSection("Year", _YearSelect, _Display);
			_YearSelect.OnChange += HandleParametersChanged;
			for (uint i = 1939; i < 1946; ++i)
				_YearSelect.Add(
					new SelectionOption<uint>(
						"scenario-builder-parameters-section-select-option")
					{
						DisplayedString = i.ToString(),
						Value = i
					});
			_YearSelect.SetValue(i => i.Value == ScenarioBuilder.Parameters.Year);

			MakeSection("Environment", _EnvironmentSelect, _Display);
			_EnvironmentSelect.OnChange += HandleParametersChanged;
			foreach (Environment environment in GameData.Environments.Values)
				_EnvironmentSelect.Add(
					new SelectionOption<Environment>("scenario-builder-parameters-section-select-option")
					{
						DisplayedString = ObjectDescriber.Describe(environment),
						Value = environment
					});
			_EnvironmentSelect.SetValue(i => i.Value == ScenarioBuilder.Parameters.Environment);

			MakeSection("Front", _FrontSelect, _Display);
			_FrontSelect.OnChange += HandleParametersChanged;
			foreach (Front front in Enum.GetValues(typeof(Front)).Cast<Front>().Skip(1))
				_FrontSelect.Add(
					new SelectionOption<Front>("scenario-builder-parameters-section-select-option")
					{
						DisplayedString = ObjectDescriber.Describe(front),
						Value = front
					});
			_FrontSelect.SetValue(i => i.Value == ScenarioBuilder.Parameters.Front);

			_Pane.Position = .5f * (WindowSize - _Pane.Size);

			Button addArmyButton = new Button("scenario-builder-army-section-add-button") { DisplayedString = "+" };
			addArmyButton.OnClick += HandleArmyAdded;
			_ArmiesTable.Add(
				new TableRow("scenario-builder-army-section-header")
				{
					new Button("scenario-builder-army-section-faction-header") { DisplayedString = "Faction"},
					new Button("scenario-builder-army-section-points-header") { DisplayedString = "Points"},
					new Button("scenario-builder-army-section-team-header") { DisplayedString = "Team"},
					addArmyButton
				});
			_Display.Add(_ArmiesTable);

			Button finishedButton = new Button("large-button") { DisplayedString = "Finished" };
			finishedButton.Position = new Vector2f(0, _Pane.Size.Y - finishedButton.Size.Y - 32);
			finishedButton.OnClick += HandleFinished;

			_Pane.Add(finishedButton);
			_Pane.Add(_Display);
			_Items.Add(_Pane);
		}

		public void AddArmyBuilder(ArmyBuilder Builder)
		{
			ScenarioBuilderArmySection section = new ScenarioBuilderArmySection(Builder, GameData.Factions.Values);
			section.OnParametersChanged += HandleArmyParametersChanged;
			section.OnRemoved += HandleArmyRemoved;
			_ArmiesTable.Add(section);
		}

		public void RemoveArmyBuilder(ArmyBuilder Builder)
		{
			_ArmiesTable.Remove(
				i => i is ScenarioBuilderArmySection && ((ScenarioBuilderArmySection)i).ArmyBuilder == Builder);
		}

		public void Alert(string Alert)
		{
			_Error.DisplayedString = Alert;
			_Display.Remove(_Error);
			_Display.Add(_Error);
		}

		public bool Validate()
		{
			return _ArmiesTable.All(
				i => !(i is ScenarioBuilderArmySection) || ((ScenarioBuilderArmySection)i).Validate());
		}

		void MakeSection(string SectionName, GuiItem Input, SingleColumnTable Display)
		{
			Button header = new Button("header-2") { DisplayedString = SectionName };
			GuiContainer<Pod> container = new GuiContainer<Pod>("scenario-builder-parameters-section");

			container.Add(Input);

			Display.Add(header);
			Display.Add(container);
		}

		void HandleParametersChanged(object Sender, EventArgs E)
		{
			if (_YearSelect.Value == null || _FrontSelect.Value == null || _EnvironmentSelect.Value == null) return;

			ScenarioParameters parameters =
				new ScenarioParameters(
					_YearSelect.Value.Value, _FrontSelect.Value.Value, _EnvironmentSelect.Value.Value);
			if (OnParametersChanged != null)
				OnParametersChanged(this, new ValuedEventArgs<ScenarioParameters>(parameters));
		}

		void HandleArmyAdded(object Sender, EventArgs E)
		{
			if (OnArmyAdded != null) OnArmyAdded(this, EventArgs.Empty);
		}

		void HandleArmyParametersChanged(object Sender, ValuedEventArgs<ArmyParameters> E)
		{
			if (OnArmyParametersChanged != null)
				OnArmyParametersChanged(
					this,
					new ValuedEventArgs<Tuple<ArmyBuilder, ArmyParameters>>(
						new Tuple<ArmyBuilder, ArmyParameters>(
							((ScenarioBuilderArmySection)Sender).ArmyBuilder, E.Value)));
		}

		void HandleArmyRemoved(object Sender, EventArgs E)
		{
			if (OnArmyRemoved != null)
				OnArmyRemoved(this, new ValuedEventArgs<ArmyBuilder>(((ScenarioBuilderArmySection)Sender).ArmyBuilder));
		}

		void HandleFinished(object Sender, EventArgs E)
		{
			if (OnFinished != null) OnFinished(this, EventArgs.Empty);
		}
	}
}
