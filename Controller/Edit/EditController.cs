using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Serialization;
using Cardamom.Utilities;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class EditController
	{
		static readonly Color HIGHLIGHT_COLOR = new Color(255, 0, 0, 120);

		EditScreen _EditScreen;
		EditPane _EditPane;
		TextPane _NewRegionPane;
		Highlight _Highlight;

		NewMapPane _NewPane = new NewMapPane { Visible = false };
		IOPane _OpenPane = new IOPane("Open") { Visible = false };
		IOPane _SavePane = new IOPane("Save") { Visible = false };

		public EditController(EditScreen EditScreen)
		{
			_EditScreen = EditScreen;
			_EditPane = new EditPane(EditScreen.MapView.Map);
			SetMap(EditScreen.MapView.Map);

			_NewPane.OnCancel += (sender, e) => _NewPane.Visible = false;
			_NewPane.OnCreate += NewMap;

			_OpenPane.SetDirectory("./Maps");
			_OpenPane.OnCancel += (sender, e) => _OpenPane.Visible = false;
			_OpenPane.OnAction += OpenMap;

			_SavePane.SetDirectory("./Maps");
			_SavePane.OnCancel += (sender, e) => _SavePane.Visible = false;
			_SavePane.OnAction += SaveMap;

			_EditScreen.OnNewClicked += (sender, e) => _NewPane.Visible = true;
			_EditScreen.OnOpenClicked += (sender, e) => _OpenPane.Visible = true;
			_EditScreen.OnSaveClicked += (sender, e) => _SavePane.Visible = true;
			_EditScreen.PaneLayer.Add(_EditPane);

			_EditScreen.PaneLayer.Add(_NewPane);
			_EditScreen.PaneLayer.Add(_SavePane);
			_EditScreen.PaneLayer.Add(_OpenPane);

			_EditPane.OnAddMapRegion += HandleStartAddMapRegion;
			_EditPane.OnDeleteMapRegion += HandleDeleteMapRegion;
			_EditPane.OnMapRegionSelected += (sender, e) => HandleRegionChanged(e.Value, EventArgs.Empty);
		}

		void Highlight(IEnumerable<Tuple<Tile, Color>> Highlight)
		{
			_EditScreen.HighlightLayer.RemoveHighlight(_Highlight);
			_Highlight = new Highlight(Highlight);
			_EditScreen.HighlightLayer.AddHighlight(_Highlight);
		}

		void UnHighlight()
		{
			Highlight(Enumerable.Empty<Tuple<Tile, Color>>());
		}

		void HandleStartAddMapRegion(object Sender, EventArgs E)
		{
			if (_NewRegionPane != null) _EditScreen.PaneLayer.Remove(_NewRegionPane);

			_NewRegionPane = new TextPane("New Region", "Region Name", "Create");
			_NewRegionPane.OnValueEntered += HandleAddMapRegion;
			_EditScreen.PaneLayer.Add(_NewRegionPane);
		}

		void HandleAddMapRegion(object Sender, ValuedEventArgs<string> E)
		{
			var m = new MapRegion { Name = E.Value };
			m.OnChange += HandleRegionChanged;
			_EditScreen.MapView.Map.Regions.Add(m);
			_EditScreen.MapView.MapRegions.Add(new MapRegionView(m, _EditScreen.MapView.TileRenderer));
			_EditPane.UpdateFromMap(_EditScreen.MapView.Map);
			UnHighlight();

			_EditScreen.PaneLayer.Remove(_NewRegionPane);
			_NewRegionPane = null;
		}

		void HandleDeleteMapRegion(object Sender, ValuedEventArgs<MapRegion> E)
		{
			_EditScreen.MapView.Map.Regions.Remove(E.Value);
			_EditScreen.MapView.MapRegions.RemoveAll(i => i.MapRegion == E.Value);
			_EditPane.UpdateFromMap(_EditScreen.MapView.Map);
		}

		void HandleRegionChanged(object Sender, EventArgs E)
		{
			if (Sender == null) return;

			var region = (MapRegion)Sender;
			Highlight(region.Tiles.Select(i => new Tuple<Tile, Color>(i, HIGHLIGHT_COLOR)));
		}

		void HandleTileClick(object Sender, MouseEventArgs E)
		{
			if (Keyboard.IsKeyPressed(Keyboard.Key.LShift))
				_EditPane.ShiftEditTile(((TileView)Sender).Tile, E.Position);
			else _EditPane.EditTile(((TileView)Sender).Tile, E.Position);
		}

		void HandleTileRightClick(object Sender, MouseEventArgs E)
		{
			if (Keyboard.IsKeyPressed(Keyboard.Key.LShift))
				_EditPane.ShiftRightEditTile(((TileView)Sender).Tile, E.Position);
			else _EditPane.RightEditTile(((TileView)Sender).Tile, E.Position);
		}

		void SetMap(Map Map)
		{
			_EditPane.UpdateFromMap(Map);
			_EditScreen.SetMap(Map);
			UnHighlight();
			foreach (MapRegion r in Map.Regions) r.OnChange += HandleRegionChanged;
			foreach (TileView t in _EditScreen.MapView.TilesEnumerable)
			{
				t.OnClick += HandleTileClick;
				t.OnRightClick += HandleTileRightClick;
			}
		}

		void NewMap(object Sender, ValuedEventArgs<MapConfiguration> E)
		{
			var pane = (NewMapPane)Sender;
			SetMap(E.Value.GenerateMap(null, new IdGenerator()));
			pane.Visible = false;
		}

		void SaveMap(object Sender, EventArgs E)
		{
			var pane = (IOPane)Sender;
			using (FileStream stream = new FileStream(pane.InputPath, FileMode.Create))
			{
				using (GZipStream compressionStream = new GZipStream(stream, CompressionLevel.Optimal))
				{
					_EditScreen.MapView.Map.Serialize(
						new SerializationOutputStream(compressionStream));
				}
			}
			pane.SetDirectory("./Maps");
			pane.Visible = false;
		}

		void OpenMap(object Sender, EventArgs E)
		{
			var pane = (IOPane)Sender;
			using (FileStream stream = new FileStream(pane.InputPath, FileMode.Open))
			{
				using (GZipStream compressionStream = new GZipStream(stream, CompressionMode.Decompress))
				{
					SetMap(
						new Map(
							new SerializationInputStream(compressionStream),
							null,
							new IdGenerator()));
				}
			}
			pane.SetDirectory("./Maps");
			pane.Visible = false;
		}
	}
}
