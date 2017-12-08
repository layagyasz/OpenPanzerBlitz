using System;
using System.IO;
using System.IO.Compression;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class MatchEndController
	{
		MatchContext _Context;
		IOPane _SavePane = new IOPane("Save") { Visible = false };

		public MatchEndController(MatchEndScreen MatchEndScreen, MatchContext Context)
		{
			_Context = Context;

			_SavePane.SetDirectory(string.Format("./MatchRecords/{0}", GameData.LoadedModule));
			_SavePane.OnCancel += (sender, e) => _SavePane.Visible = false;
			_SavePane.OnAction += SaveMatchRecord;

			MatchEndScreen.OnSaveClicked += (sender, e) => _SavePane.Visible = true;
			MatchEndScreen.PaneLayer.Add(_SavePane);
		}

		void SaveMatchRecord(object Sender, EventArgs E)
		{
			IOPane pane = (IOPane)Sender;
			using (FileStream stream = new FileStream(pane.InputPath, FileMode.Create))
			{
				using (GZipStream compressionStream = new GZipStream(stream, CompressionLevel.Optimal))
				{
					new MatchRecord(_Context.Match, _Context.OrderSerializer).Serialize(
						new SerializationOutputStream(compressionStream));
				}
			}

			_SavePane.SetDirectory(string.Format("./MatchRecords/{0}", GameData.LoadedModule));
			_SavePane.Visible = false;
		}
	}
}
