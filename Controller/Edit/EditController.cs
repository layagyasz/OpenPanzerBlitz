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
		GameScreen _GameScreen;
		EditPane _EditPane = new EditPane();

		IOPane _OpenPane = new IOPane("Open") { Visible = false };
		IOPane _SavePane = new IOPane("Save") { Visible = false };
		DropDown<object> _FileDropDown = new DropDown<object>("select");

		public EditController(GameScreen GameScreen)
		{
			_FileDropDown.Add(new SelectionOption<object>("select-option") { DisplayedString = "File" });

			SelectionOption<object> openOption =
				new SelectionOption<object>("select-option") { DisplayedString = "Open" };
			openOption.OnClick += (sender, e) => _OpenPane.Visible = true;
			_FileDropDown.Add(openOption);

			SelectionOption<object> saveOption =
				new SelectionOption<object>("select-option") { DisplayedString = "Save" };
			saveOption.OnClick += (sender, e) => _SavePane.Visible = true;
			_FileDropDown.Add(saveOption);

			_OpenPane.SetDirectory("./Maps");
			_OpenPane.OnCancel += (sender, e) => _OpenPane.Visible = false;
			_OpenPane.OnAction += (sender, e) => Open(sender, e);

			_SavePane.SetDirectory("./Maps");
			_SavePane.OnCancel += (sender, e) => _SavePane.Visible = false;
			_SavePane.OnAction += (sender, e) => Save(sender, e);

			_GameScreen = GameScreen;
			_GameScreen.AddPane(_EditPane);

			foreach (TileView t in _GameScreen.MapView.TilesEnumerable)
			{
				t.OnClick += OnTileClick;
				t.OnRightClick += OnTileRightClick;
			}

			_GameScreen.AddItem(_FileDropDown);
			_GameScreen.AddPane(_SavePane);
			_GameScreen.AddPane(_OpenPane);
		}

		private void OnTileClick(object sender, MouseEventArgs e)
		{
			_EditPane.EditTile(((TileView)sender).Tile, e.Position);
		}

		private void OnTileRightClick(object sender, MouseEventArgs e)
		{
			_EditPane.RightEditTile(((TileView)sender).Tile, e.Position);
		}

		private void Save(object sender, EventArgs e)
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

		private void Open(object sender, EventArgs e)
		{
			IOPane pane = (IOPane)sender;
			using (FileStream stream = new FileStream(pane.InputPath, FileMode.Open))
			{
				using (GZipStream compressionStream = new GZipStream(stream, CompressionMode.Decompress))
				{
					_GameScreen.SetMapView(
						new MapView(
							new Map(new SerializationInputStream(compressionStream)), TileRenderer.SUMMER_STEPPE));
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
