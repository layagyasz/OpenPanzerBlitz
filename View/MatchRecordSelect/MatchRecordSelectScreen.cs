using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public class MatchRecordSelectScreen : ScreenBase
	{
		public EventHandler<ValuedEventArgs<FileInfo>> OnMatchRecordSelected;

		ValuedScrollCollection<SelectionOption<FileInfo>, FileInfo> _MatchRecordSelect =
			new ValuedScrollCollection<SelectionOption<FileInfo>, FileInfo>("match-record-select");

		public MatchRecordSelectScreen(Vector2f WindowSize, string Path)
			: base(WindowSize)
		{
			if (Directory.Exists(Path))
			{
				foreach (FileInfo file in Directory.EnumerateFiles(Path).Select(i => new FileInfo(i)))
				{
					SelectionOption<FileInfo> option = new SelectionOption<FileInfo>("match-record-selection-option")
					{
						DisplayedString = file.Name,
						Value = file
					};
					_MatchRecordSelect.Add(option);
				}
			}
			_MatchRecordSelect.OnChange += HandleSelect;
			_MatchRecordSelect.Position = .5f * (WindowSize - _MatchRecordSelect.Size);

			_Items.Add(_MatchRecordSelect);
		}

		void HandleSelect(object Sender, ValuedEventArgs<SelectionOption<FileInfo>> E)
		{
			if (OnMatchRecordSelected != null)
				OnMatchRecordSelected(this, new ValuedEventArgs<FileInfo>(E.Value.Value));
		}
	}
}
