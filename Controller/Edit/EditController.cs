using System;
using System.IO;
using System.IO.Compression;

using Cardamom.Interface;
using Cardamom.Interface.Items;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class EditController
	{
		EditScreen _GameScreen;
		EditPane _EditPane = new EditPane();

		IOPane _OpenPane = new IOPane("Open") { Visible = false };
		IOPane _SavePane = new IOPane("Save") { Visible = false };

		public EditController(EditScreen GameScreen)
		{
			_OpenPane.SetDirectory("./Maps");
			_OpenPane.OnCancel += (sender, e) => _OpenPane.Visible = false;
			_OpenPane.OnAction += (sender, e) => Open(sender, e);

			_SavePane.SetDirectory("./Maps");
			_SavePane.OnCancel += (sender, e) => _SavePane.Visible = false;
			_SavePane.OnAction += (sender, e) => Save(sender, e);

			_GameScreen = GameScreen;
			_GameScreen.OnOpenClicked += (sender, e) => _OpenPane.Visible = true;
			_GameScreen.OnSaveClicked += (sender, e) => _SavePane.Visible = true;
			_GameScreen.PaneLayer.Add(_EditPane);

			foreach (TileView t in _GameScreen.MapView.TilesEnumerable)
			{
				t.OnClick += OnTileClick;
				t.OnRightClick += OnTileRightClick;
			}

			_GameScreen.PaneLayer.Add(_SavePane);
			_GameScreen.PaneLayer.Add(_OpenPane);
		}

		void OnTileClick(object sender, MouseEventArgs e)
		{
			_EditPane.EditTile(((TileView)sender).Tile, e.Position);
		}

		void OnTileRightClick(object sender, MouseEventArgs e)
		{
			_EditPane.RightEditTile(((TileView)sender).Tile, e.Position);
		}

		void Save(object sender, EventArgs e)
		{
			IOPane pane = (IOPane)sender;
			using (FileStream stream = new FileStream(pane.InputPath, FileMode.Create))
			{
				using (GZipStream compressionStream = new GZipStream(stream, CompressionLevel.Optimal))
				{
					_GameScreen.MapView.Map.Serialize(
						new SerializationOutputStream(compressionStream));
				}
			}
			pane.SetDirectory("./Maps");
			pane.Visible = false;
		}

		void Open(object sender, EventArgs e)
		{
			IOPane pane = (IOPane)sender;
			using (FileStream stream = new FileStream(pane.InputPath, FileMode.Open))
			{
				using (GZipStream compressionStream = new GZipStream(stream, CompressionMode.Decompress))
				{
					_GameScreen.SetMap(new Map(new SerializationInputStream(compressionStream)));
					foreach (TileView t in _GameScreen.MapView.TilesEnumerable)
					{
						t.OnClick += OnTileClick;
						t.OnRightClick += OnTileRightClick;
					}

				}
			}
			pane.SetDirectory("./Maps");
			pane.Visible = false;
		}
	}
}
