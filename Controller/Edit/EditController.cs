using System;
using System.IO;
using System.IO.Compression;

using Cardamom.Interface;
using Cardamom.Serialization;
using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public class EditController
	{
		EditScreen _GameScreen;
		EditPane _EditPane = new EditPane();

		NewMapPane _NewPane = new NewMapPane() { Visible = false };
		IOPane _OpenPane = new IOPane("Open") { Visible = false };
		IOPane _SavePane = new IOPane("Save") { Visible = false };

		public EditController(EditScreen GameScreen)
		{
			_NewPane.OnCancel += (sender, e) => _NewPane.Visible = false;
			_NewPane.OnCreate += New;

			_OpenPane.SetDirectory("./Maps");
			_OpenPane.OnCancel += (sender, e) => _OpenPane.Visible = false;
			_OpenPane.OnAction += (sender, e) => Open(sender, e);

			_SavePane.SetDirectory("./Maps");
			_SavePane.OnCancel += (sender, e) => _SavePane.Visible = false;
			_SavePane.OnAction += (sender, e) => Save(sender, e);

			_GameScreen = GameScreen;
			_GameScreen.OnNewClicked += (sender, e) => _NewPane.Visible = true;
			_GameScreen.OnOpenClicked += (sender, e) => _OpenPane.Visible = true;
			_GameScreen.OnSaveClicked += (sender, e) => _SavePane.Visible = true;
			_GameScreen.PaneLayer.Add(_EditPane);

			foreach (TileView t in _GameScreen.MapView.TilesEnumerable)
			{
				t.OnClick += OnTileClick;
				t.OnRightClick += OnTileRightClick;
			}

			_GameScreen.PaneLayer.Add(_NewPane);
			_GameScreen.PaneLayer.Add(_SavePane);
			_GameScreen.PaneLayer.Add(_OpenPane);
		}

		void OnTileClick(object Sender, MouseEventArgs E)
		{
			_EditPane.EditTile(((TileView)Sender).Tile, E.Position);
		}

		void OnTileRightClick(object Sender, MouseEventArgs E)
		{
			_EditPane.RightEditTile(((TileView)Sender).Tile, E.Position);
		}

		void New(object Sender, ValuedEventArgs<Vector2i> E)
		{
			NewMapPane pane = (NewMapPane)Sender;
			Map newMap = new RandomMapConfiguration(E.Value.X, E.Value.Y).GenerateMap(new IdGenerator());
			_GameScreen.SetMap(newMap);
			pane.Visible = false;
		}

		void Save(object Sender, EventArgs E)
		{
			IOPane pane = (IOPane)Sender;
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

		void Open(object Sender, EventArgs E)
		{
			IOPane pane = (IOPane)Sender;
			using (FileStream stream = new FileStream(pane.InputPath, FileMode.Open))
			{
				using (GZipStream compressionStream = new GZipStream(stream, CompressionMode.Decompress))
				{
					_GameScreen.SetMap(
						new Map(
							new SerializationInputStream(compressionStream),
							null,
							new IdGenerator()));
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
