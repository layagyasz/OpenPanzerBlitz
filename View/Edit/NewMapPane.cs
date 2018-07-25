using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public class NewMapPane : Pane
	{
		public EventHandler<EventArgs> OnCancel;
		public EventHandler<ValuedEventArgs<MapConfiguration>> OnCreate;

		readonly SingleColumnTable _Display = new SingleColumnTable("new-map-display");
		readonly TextInput _HeightInput = new TextInput("new-map-text-input");
		readonly TextInput _WidthInput = new TextInput("new-map-text-input");
		readonly Checkbox _GenerateRandomCheckbox =
			new Checkbox("new-map-checkbox") { DisplayedString = "Generate Random" };
		readonly Select<MatchSetting> _MatchSettingsSelect =
			new Select<MatchSetting>("new-map-parameters-section-select");
		readonly Button _Error = new Button("new-map-error");
		readonly Button _CancelButton = new Button("small-button") { DisplayedString = "Cancel" };
		readonly Button _CreateButton = new Button("small-button") { DisplayedString = "Create" };

		public NewMapPane()
			: base("new-map-pane")
		{
			_CreateButton.Position = Size - new Vector2f(_CreateButton.Size.X + 32, _CreateButton.Size.Y + 32);
			_CancelButton.Position = _CreateButton.Position - new Vector2f(_CancelButton.Size.X + 4, 0);

			_CancelButton.OnClick += HandleCancelClicked;
			_CreateButton.OnClick += HandleCreateClicked;

			foreach (var matchSetting in GameData.MatchSettings.Values)
				_MatchSettingsSelect.Add(
					new SelectionOption<MatchSetting>("new-map-parameters-section-select-option")
					{
						Value = matchSetting,
						DisplayedString = ObjectDescriber.Describe(matchSetting)
					});

			_Display.Add(new Button("new-map-header-1") { DisplayedString = "New Map" });
			_Display.Add(new Button("new-map-header-2") { DisplayedString = "Height" });
			_Display.Add(_HeightInput);
			_Display.Add(new Button("new-map-header-2") { DisplayedString = "Width" });
			_Display.Add(_WidthInput);
			_Display.Add(new Button("new-map-header-2") { DisplayedString = "Random Map Generation" });
			_Display.Add(_GenerateRandomCheckbox);
			_Display.Add(new GuiContainer<Pod>("new-map-parameters-section") { _MatchSettingsSelect });

			Add(_Display);
			Add(_CancelButton);
			Add(_CreateButton);
		}

		public void SetError(string Message)
		{
			_Error.DisplayedString = Message;
			_Display.Remove(_Error);
			_Display.Add(_Error);		}

		public void ClearError()
		{
			_Display.Remove(_Error);
		}

		void HandleCancelClicked(object Sender, EventArgs E)
		{
			if (OnCancel != null) OnCancel(this, E);
		}

		void HandleCreateClicked(object Sender, EventArgs E)
		{
			try
			{
				var height = Convert.ToInt32(_HeightInput.Value);
				var width = Convert.ToInt32(_WidthInput.Value);
				if (height < 1 || width < 1) throw new FormatException();

				MapConfiguration configuration = null;
				if (_GenerateRandomCheckbox.Value)
					configuration = new RandomMapConfiguration(width, height, _MatchSettingsSelect.Value.Value);
				else configuration = new BlankMapConfiguration(width, height);
				if (OnCreate != null) OnCreate(this, new ValuedEventArgs<MapConfiguration>(configuration));

				ClearError();
			}
			catch (FormatException)
			{
				SetError("Input a number greater than 0.");
			}
		}
	}
}
