using System;
using System.IO;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public class IOPane : Pane
	{
		public EventHandler<EventArgs> OnAction;
		public EventHandler<EventArgs> OnCancel;

		readonly ValuedScrollCollection<SelectionOption<FileInfo>, FileInfo> _FileSelect =
			new ValuedScrollCollection<SelectionOption<FileInfo>, FileInfo>("io-select");
		readonly Button _ActionButton = new Button("small-button");
		readonly Button _CancelButton = new Button("small-button");
		readonly TextInput _FileNameInput = new TextInput("io-text-input");

		string _Directory;

		public string InputPath
		{
			get
			{
				return _Directory + '/' + _FileNameInput.Value;
			}
		}

		public IOPane(string ActionText)
			: base("io-pane")
		{
			_CancelButton.OnClick += (sender, e) => { if (OnCancel != null) OnCancel(this, EventArgs.Empty); };
			_ActionButton.OnClick += (sender, e) => { if (OnAction != null) OnAction(this, EventArgs.Empty); };

			_CancelButton.DisplayedString = "Cancel";
			_ActionButton.DisplayedString = ActionText;

			_FileNameInput.Position = new Vector2f(0, _FileSelect.Size.Y + 4);
			_ActionButton.Position = _FileNameInput.Position
				+ _FileNameInput.Size
				+ new Vector2f(-_ActionButton.Size.X, 4);
			_CancelButton.Position = _ActionButton.Position - new Vector2f(_CancelButton.Size.X + 4, 0);

			Add(_FileNameInput);
			Add(_FileSelect);
			Add(_CancelButton);
			Add(_ActionButton);
		}

		public void SetDirectory(string Path)
		{
			_Directory = Path;
			_FileSelect.Clear();

			if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);

			foreach (FileInfo f in Directory.EnumerateFiles(Path).Select(i => new FileInfo(i)))
			{
				var option =
					new SelectionOption<FileInfo>("io-select-option") { DisplayedString = f.Name, Value = f };
				option.OnClick += (sender, e) => _FileNameInput.Value = ((SelectionOption<FileInfo>)sender).Value.Name;
				_FileSelect.Add(option);
			}
		}
	}
}
