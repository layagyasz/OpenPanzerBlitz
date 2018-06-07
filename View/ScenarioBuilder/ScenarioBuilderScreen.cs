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
		SingleColumnTable _LeftDisplay = new SingleColumnTable("scenario-builder-display");
		Button _Error = new Button("footer-error");

		Select<uint> _YearSelect = new Select<uint>("scenario-builder-parameters-section-select");
		Select<MatchSetting> _SettingSelect = new Select<MatchSetting>("scenario-builder-parameters-section-select");
		Select<byte> _TurnsSelect = new Select<byte>("scenario-builder-parameters-section-select");
		Checkbox _FogOfWarCheckbox =
			new Checkbox("scenario-builder-parameters-section-checkbox") { DisplayedString = "Enable" };
		TextInput _MapWidthInput = new TextInput("scenario-builder-parameters-section-text-input");
		TextInput _MapHeightInput = new TextInput("scenario-builder-parameters-section-text-input");

		Table _ArmiesTable = new Table("scenario-builder-army-section-table", true);

		public ScenarioBuilderScreen(Vector2f WindowSize, ScenarioBuilder ScenarioBuilder)
			: base(WindowSize, true)
		{
			this.ScenarioBuilder = ScenarioBuilder;

			var header = new Button("scenario-builder-header") { DisplayedString = "Custom Scenario" };
			_Pane.Add(header);
			_LeftDisplay.Position = new Vector2f(0, header.Size.Y);
			_ArmiesTable.Position = new Vector2f(_LeftDisplay.Size.X + 32, header.Size.Y);

			MakeSection("Year", _YearSelect, _LeftDisplay);
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

			MakeSection("Setting", _SettingSelect, _LeftDisplay);
			_SettingSelect.OnChange += HandleParametersChanged;
			foreach (MatchSetting setting in GameData.MatchSettings.Values)
				_SettingSelect.Add(
					new SelectionOption<MatchSetting>("scenario-builder-parameters-section-select-option")
					{
						DisplayedString = ObjectDescriber.Describe(setting),
						Value = setting
					});
			_SettingSelect.SetValue(i => i.Value == ScenarioBuilder.Parameters.Setting);

			MakeSection("Turns", _TurnsSelect, _LeftDisplay);
			_TurnsSelect.OnChange += HandleParametersChanged;
			for (byte i = 6; i <= 16; i += 2)
				_TurnsSelect.Add(
					new SelectionOption<byte>("scenario-builder-parameters-section-select-option")
					{
						DisplayedString = i.ToString(),
						Value = i
					});
			_TurnsSelect.SetValue(i => i.Value == ScenarioBuilder.Parameters.Turns);

			MakeSection("Fog of War", _FogOfWarCheckbox, _LeftDisplay);
			_FogOfWarCheckbox.OnChange += HandleParametersChanged;
			_FogOfWarCheckbox.Value = ScenarioBuilder.Parameters.FogOfWar;

			MakeSection("Map Width", _MapWidthInput, _LeftDisplay);
			_MapWidthInput.OnChange += HandleParametersChanged;
			_MapWidthInput.Value = ScenarioBuilder.Parameters.MapSize.X.ToString();

			MakeSection("Map Height", _MapHeightInput, _LeftDisplay);
			_MapHeightInput.OnChange += HandleParametersChanged;
			_MapHeightInput.Value = ScenarioBuilder.Parameters.MapSize.Y.ToString();

			_Pane.Position = .5f * (WindowSize - _Pane.Size);

			var addArmyButton = new Button("scenario-builder-army-section-add-button") { DisplayedString = "+" };
			addArmyButton.OnClick += HandleArmyAdded;
			_ArmiesTable.Add(
				new TableRow("scenario-builder-army-section-header")
				{
					new Button("scenario-builder-army-section-faction-header") { DisplayedString = "Faction"},
					new Button("scenario-builder-army-section-points-header") { DisplayedString = "Points"},
					new Button("scenario-builder-army-section-team-header") { DisplayedString = "Team"},
					addArmyButton
				});

			var finishedButton = new Button("large-button") { DisplayedString = "Finished" };
			finishedButton.Position = new Vector2f(0, _Pane.Size.Y - finishedButton.Size.Y - 32);
			finishedButton.OnClick += HandleFinished;

			_Pane.Add(finishedButton);
			_Pane.Add(_LeftDisplay);
			_Pane.Add(_ArmiesTable);
			_Items.Add(_Pane);
		}

		public void AddArmyBuilder(ArmyBuilder Builder)
		{
			var section = new ScenarioBuilderArmySection(Builder, GameData.Factions.Values);
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
			_LeftDisplay.Remove(_Error);
			_LeftDisplay.Add(_Error);
		}

		public bool Validate()
		{
			return ValidateIntegerInput(_MapWidthInput.Value) > 0
				&& ValidateIntegerInput(_MapHeightInput.Value) > 0
				&& _ArmiesTable.All(
					i => !(i is ScenarioBuilderArmySection) || ((ScenarioBuilderArmySection)i).Validate());
		}

		void MakeSection(string SectionName, GuiItem Input, SingleColumnTable Display)
		{
			var header = new Button("header-2") { DisplayedString = SectionName };
			var container = new GuiContainer<Pod>("scenario-builder-parameters-section");

			container.Add(Input);

			Display.Add(header);
			Display.Add(container);
		}

		int ValidateIntegerInput(string Value)
		{
			try
			{
				return Convert.ToInt32(Value);
			}
			catch (Exception e)
			{
				return 0;
			}
		}

		void HandleParametersChanged(object Sender, EventArgs E)
		{
			if (_YearSelect.Value == null
				|| _SettingSelect.Value == null
				|| _TurnsSelect.Value == null)
				return;

			var width = ValidateIntegerInput(_MapWidthInput.Value);
			var height = ValidateIntegerInput(_MapHeightInput.Value);

			if (width == 0 || height == 0)
				return;

			var parameters =
				new ScenarioParameters(
					_YearSelect.Value.Value,
					_SettingSelect.Value.Value,
					_TurnsSelect.Value.Value,
					new Coordinate(width, height),
					_FogOfWarCheckbox.Value);
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
