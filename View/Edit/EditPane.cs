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
		Select<TileEdge> _EdgeSelect = new Select<TileEdge>("select");
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

			Button header = new Button("edit-header-1") { DisplayedString = "Edit" };
			_ModeSelect.Position = new Vector2f(0, header.Size.Y);
			_TileBaseSelect.Position = new Vector2f(0, _ModeSelect.Position.Y + _ModeSelect.Size.Y + 2);
			_PathOverlaySelect.Position = new Vector2f(0, _ModeSelect.Position.Y + _ModeSelect.Size.Y + 2);
			_EdgeSelect.Position = new Vector2f(0, _ModeSelect.Position.Y + _ModeSelect.Size.Y + 2);
			_Elevation.Position = new Vector2f(0, _ModeSelect.Position.Y + _ModeSelect.Size.Y + 2);

			foreach (TileBase t in Enum.GetValues(typeof(TileBase)).Cast<TileBase>().Where(i => i != TileBase.CLEAR))
				_TileBaseSelect.Add(
					new SelectionOption<TileBase>("select-option")
					{
						DisplayedString = ObjectDescriber.Describe(t),
						Value = t
					});

			foreach (TileEdge e in Enum.GetValues(typeof(TileEdge)).Cast<TileEdge>().Where(i => i != TileEdge.NONE))
				_EdgeSelect.Add(new SelectionOption<TileEdge>("select-option")
				{
					DisplayedString = ObjectDescriber.Describe(e),
					Value = e
				});

			foreach (TilePathOverlay t in Enum.GetValues(typeof(TilePathOverlay))
					 .Cast<TilePathOverlay>().Where(i => i != TilePathOverlay.NONE))
				_PathOverlaySelect.Add(new SelectionOption<TilePathOverlay>("select-option")
				{
					DisplayedString = ObjectDescriber.Describe(t),
					Value = t
				});

			Add(header);
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
				Tile.Configuration.SetTileBase(_TileBaseSelect.Value.Value);
			}
			else if (_ModeSelect.Value.Value == _PathOverlaySelect)
			{
				int index = Enumerable.Range(0, 6).ArgMax(i => -Tile.Bounds[i].DistanceSquared(Point));
				Tile.SetPathOverlay(index, _PathOverlaySelect.Value.Value);
			}
			else if (_ModeSelect.Value.Value == _Elevation)
			{
				Tile.Configuration.SetElevation(Tile.Configuration.Elevation + 1);
			}
		}

		public void RightEditTile(Tile Tile, Vector2f Point)
		{
			if (_ModeSelect.Value.Value == _EdgeSelect)
			{
				int index = Enumerable.Range(0, 6).ArgMax(i => -Tile.Bounds[i].DistanceSquared(Point));
				Tile.SetEdge(index, TileEdge.NONE);
			}
			else if (_ModeSelect.Value.Value == _TileBaseSelect)
			{
				Tile.Configuration.SetTileBase(TileBase.CLEAR);
			}
			else if (_ModeSelect.Value.Value == _PathOverlaySelect)
			{
				int index = Enumerable.Range(0, 6).ArgMax(i => -Tile.Bounds[i].DistanceSquared(Point));
				Tile.SetPathOverlay(index, TilePathOverlay.NONE);
			}
			else if (_ModeSelect.Value.Value == _Elevation)
			{
				Tile.Configuration.SetElevation(Tile.Configuration.Elevation - 1);
			}
		}
	}
}
