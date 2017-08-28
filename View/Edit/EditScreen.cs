using System;

using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public class EditScreen : MapScreenBase
	{
		public EventHandler<EventArgs> OnNewClicked;
		public EventHandler<EventArgs> OnOpenClicked;
		public EventHandler<EventArgs> OnSaveClicked;

		DropDown<object> _FileDropDown = new DropDown<object>("select");

		public EditScreen(Vector2f WindowSize, Map Map, TileRenderer TileRenderer)
			: base(WindowSize, Map, TileRenderer)
		{
			_FileDropDown.Add(new SelectionOption<object>("select-option") { DisplayedString = "File" });

			SelectionOption<object> newOption =
				new SelectionOption<object>("select-option") { DisplayedString = "New " };
			newOption.OnClick += HandleNewClicked;
			_FileDropDown.Add(newOption);

			SelectionOption<object> openOption =
				new SelectionOption<object>("select-option") { DisplayedString = "Open" };
			openOption.OnClick += HandleOpenClicked;
			_FileDropDown.Add(openOption);

			SelectionOption<object> saveOption =
				new SelectionOption<object>("select-option") { DisplayedString = "Save" };
			saveOption.OnClick += HandleSaveClicked;
			_FileDropDown.Add(saveOption);

			_Items.Add(_FileDropDown);
		}

		void HandleNewClicked(object Sender, EventArgs E)
		{
			if (OnNewClicked != null) OnNewClicked(this, E);
		}

		void HandleOpenClicked(object Sender, EventArgs E)
		{
			if (OnOpenClicked != null) OnOpenClicked(this, E);
		}

		void HandleSaveClicked(object Sender, EventArgs E)
		{
			if (OnSaveClicked != null) OnSaveClicked(this, E);
		}
	}
}
