using System;

using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public class NewMapPane : Pane
	{
		public EventHandler<EventArgs> OnCancel;
		public EventHandler<ValuedEventArgs<Vector2i>> OnCreate;

		SingleColumnTable _Display = new SingleColumnTable("new-map-display");
		TextInput _HeightInput = new TextInput("new-map-text-input");
		TextInput _WidthInput = new TextInput("new-map-text-input");
		Button _Error = new Button("new-map-error");
		Button _CancelButton = new Button("small-button") { DisplayedString = "Cancel" };
		Button _CreateButton = new Button("small-button") { DisplayedString = "Create" };

		public NewMapPane()
			: base("new-map-pane")
		{
			_CreateButton.Position = Size - new Vector2f(_CreateButton.Size.X + 32, _CreateButton.Size.Y + 32);
			_CancelButton.Position = _CreateButton.Position - new Vector2f(_CancelButton.Size.X + 4, 0);

			_CancelButton.OnClick += HandleCancelClicked;
			_CreateButton.OnClick += HandleCreateClicked;

			_Display.Add(new Button("new-map-header-1") { DisplayedString = "New Map" });
			_Display.Add(new Button("new-map-header-2") { DisplayedString = "Height" });
			_Display.Add(_HeightInput);
			_Display.Add(new Button("new-map-header-2") { DisplayedString = "Width" });
			_Display.Add(_WidthInput);

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
				int height = Convert.ToInt32(_HeightInput.Value);
				int width = Convert.ToInt32(_WidthInput.Value);
				if (height < 1 || width < 1) throw new FormatException();

				Vector2i size = new Vector2i(width, height);
				if (OnCreate != null) OnCreate(this, new ValuedEventArgs<Vector2i>(size));

				ClearError();
			}
			catch (FormatException)
			{
				SetError("Input a number greater than 0.");
			}
		}
	}
}
