using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;
using Cardamom.Serialization;
using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public class MatchRecordSelectScreen : ScreenBase
	{
		public EventHandler<ValuedEventArgs<MatchRecord>> OnMatchRecordSelected;

		GuiContainer<GuiItem> _Display = new GuiContainer<GuiItem>("match-record-select-display");

		ValuedScrollCollection<SelectionOption<FileInfo>, FileInfo> _MatchRecordSelect =
			new ValuedScrollCollection<SelectionOption<FileInfo>, FileInfo>("match-record-select");
		ScenarioView _ScenarioView = new ScenarioView();
		Button _StartButton = new Button("large-button") { DisplayedString = "Play" };

		MatchRecord _SelectedRecord;

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
			_ScenarioView.Position = new Vector2f(_MatchRecordSelect.Size.X + 16, 0);

			_StartButton.OnClick += HandleStart;
			_StartButton.Position = new Vector2f(0, _Display.Size.Y - _StartButton.Size.Y - 32);

			_Display.Position = .5f * (WindowSize - _Display.Size);

			_Display.Add(_MatchRecordSelect);
			_Display.Add(_ScenarioView);
			_Display.Add(_StartButton);
			_Items.Add(_Display);
		}

		void HandleSelect(object Sender, ValuedEventArgs<SelectionOption<FileInfo>> E)
		{
			MatchRecord record = null;
			using (FileStream stream = new FileStream(E.Value.Value.FullName, FileMode.Open))
			{
				using (GZipStream compressionStream = new GZipStream(stream, CompressionMode.Decompress))
				{
					record = new MatchRecord(new SerializationInputStream(compressionStream));
				}
			}
			_ScenarioView.SetScenario(
				record.Match.Scenario,
				new UnitConfigurationRenderer(
					record.Match.Scenario,
					GameData.UnitRenderDetails,
					128,
					1024,
					ClassLibrary.Instance.GetFont("compacta")),
				new FactionRenderer(
					record.Match.Scenario,
					GameData.FactionRenderDetails,
					512,
					1024));
			_SelectedRecord = record;
		}

		void HandleStart(object Sender, EventArgs E)
		{
			if (OnMatchRecordSelected != null && _MatchRecordSelect.Value != null)
				OnMatchRecordSelected(this, new ValuedEventArgs<MatchRecord>(_SelectedRecord));
		}
	}
}
