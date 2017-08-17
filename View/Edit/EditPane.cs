using System;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public class EditPane : Pane
	{
		Select<GuiItem> _ModeSelect = new Select<GuiItem>("select");

		Select<TileBase> _TileBaseSelect = new Select<TileBase>("select");
		Select<Edge> _EdgeSelect = new Select<Edge>("select");
		Select<TilePathOverlay> _PathOverlaySelect = new Select<TilePathOverlay>("select");
		Button _Elevation = new Button("select-option") { DisplayedString = "Left/Right Click" };

		public EditPane()
			: base("edit-pane")
		{
			_ModeSelect.OnChange += PaneChange;
			_ModeSelect.Add(
				new SelectionOption<GuiItem>("select-option")
				{
					DisplayedString = "Base",
					Value = _TileBaseSelect
				});
			_ModeSelect.Add(
				new SelectionOption<GuiItem>("select-option")
				{
					DisplayedString = "Overlay",
					Value = _PathOverlaySelect
				});
			_ModeSelect.Add(
				new SelectionOption<GuiItem>("select-option") { DisplayedString = "Edge", Value = _EdgeSelect });
			_ModeSelect.Add(
				new SelectionOption<GuiItem>("select-option") { DisplayedString = "Elevation", Value = _Elevation });

			_TileBaseSelect.Position = new Vector2f(0, _ModeSelect.Size.Y + 2);
			_PathOverlaySelect.Position = new Vector2f(0, _ModeSelect.Size.Y + 2);
			_EdgeSelect.Position = new Vector2f(0, _ModeSelect.Size.Y + 2);
			_Elevation.Position = new Vector2f(0, _ModeSelect.Size.Y + 2);

			foreach (TileBase t in TileBase.TILE_BASES)
				_TileBaseSelect.Add(
					new SelectionOption<TileBase>("select-option") { DisplayedString = t.Name, Value = t });

			foreach (Edge e in Edge.EDGES.Where(i => i != null))
				_EdgeSelect.Add(
					new SelectionOption<Edge>("select-option") { DisplayedString = e.Name, Value = e });

			foreach (TilePathOverlay t in TilePathOverlay.PATH_OVERLAYS.Where(i => i != null))
				_PathOverlaySelect.Add(new SelectionOption<TilePathOverlay>("select-option")
				{
					DisplayedString = t.Name,
					Value = t
				});

			Add(_Elevation);
			Add(_PathOverlaySelect);
			Add(_EdgeSelect);
			Add(_TileBaseSelect);
			Add(_ModeSelect);
		}

		private void PaneChange(object Sender, EventArgs E)
		{
			GuiItem select = ((Select<GuiItem>)Sender).Value.Value;

			_TileBaseSelect.Visible = false;
			_EdgeSelect.Visible = false;
			_PathOverlaySelect.Visible = false;
			_Elevation.Visible = false;

			select.Visible = true;
		}

		public void EditTile(Tile Tile, Vector2f Point)
		{
			if (_ModeSelect.Value.Value == _EdgeSelect)
			{
				int index = Enumerable.Range(0, 6).ArgMax(i => -Tile.Bounds[i].DistanceSquared(Point));
				Tile.SetEdge(index, _EdgeSelect.Value.Value);
			}
			else if (_ModeSelect.Value.Value == _TileBaseSelect)
			{
				Tile.TileBase = _TileBaseSelect.Value.Value;
			}
			else if (_ModeSelect.Value.Value == _PathOverlaySelect)
			{
				int index = Enumerable.Range(0, 6).ArgMax(i => -Tile.Bounds[i].DistanceSquared(Point));
				Tile.SetPathOverlay(index, _PathOverlaySelect.Value.Value);
			}
			else if (_ModeSelect.Value.Value == _Elevation)
			{
				Tile.Elevation++;
			}
		}

		public void RightEditTile(Tile Tile, Vector2f Point)
		{
			if (_ModeSelect.Value.Value == _EdgeSelect)
			{
				int index = Enumerable.Range(0, 6).ArgMax(i => -Tile.Bounds[i].DistanceSquared(Point));
				Tile.SetEdge(index, null);
			}
			else if (_ModeSelect.Value.Value == _TileBaseSelect)
			{
				Tile.TileBase = TileBase.CLEAR;
			}
			else if (_ModeSelect.Value.Value == _PathOverlaySelect)
			{
				int index = Enumerable.Range(0, 6).ArgMax(i => -Tile.Bounds[i].DistanceSquared(Point));
				Tile.SetPathOverlay(index, null);
			}
			else if (_ModeSelect.Value.Value == _Elevation)
			{
				Tile.Elevation--;
			}
		}
	}
}
