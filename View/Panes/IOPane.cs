using System;
using System.IO;
using System.Linq;

using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public class IOPane : Pane
	{
		public EventHandler<EventArgs> OnAction;
		public EventHandler<EventArgs> OnCancel;

		Select<FileInfo> _FileSelect = new Select<FileInfo>("io-select", false);
		Button _ActionButton = new Button("small-button");
		Button _CancelButton = new Button("small-button");

		public IOPane(string ActionText)
			: base("io-pane")
		{
			_CancelButton.OnClick += (sender, e) => { if (OnCancel != null) OnCancel(this, EventArgs.Empty); };
			_ActionButton.OnClick += (sender, e) => { if (OnCancel != null) OnAction(this, EventArgs.Empty); };

			_CancelButton.DisplayedString = "Cancel";
			_ActionButton.DisplayedString = ActionText;

			_CancelButton.Position = _FileSelect.Size
				+ new Vector2f(-(_CancelButton.Size.X + _ActionButton.Size.X + 4), 4);
			_ActionButton.Position = _CancelButton.Position + new Vector2f(_CancelButton.Size.X + 4, 0);

			Add(_FileSelect);
			Add(_CancelButton);
			Add(_ActionButton);
		}

		public void SetDirectory(string Path)
		{
			_FileSelect.Clear();
			foreach (FileInfo f in Directory.EnumerateFiles(Path).Select(i => new FileInfo(i)))
				_FileSelect.Add(
					new SelectionOption<FileInfo>("io-select-option") { DisplayedString = f.Name, Value = f });
		}
	}
}
