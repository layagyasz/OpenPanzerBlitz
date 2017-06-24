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
				new SelectionOption<GuiItem>("select-option") { DisplayedString = "Edge", Value = _EdgeSelect });
			_ModeSelect.Add(
				new SelectionOption<GuiItem>("select-option") { DisplayedString = "Elevation", Value = _Elevation });

			_TileBaseSelect.Position = new Vector2f(0, _ModeSelect.Size.Y + 2);
			_EdgeSelect.Position = new Vector2f(0, _ModeSelect.Size.Y + 2);
			_Elevation.Position = new Vector2f(0, _ModeSelect.Size.Y + 2);

			foreach (TileBase t in
					 new TileBase[]
			{
				TileBase.CLEAR,
				TileBase.SLOPE,
				TileBase.SWAMP,
				TileBase.WATER
			})
				_TileBaseSelect.Add(
					new SelectionOption<TileBase>("select-option") { DisplayedString = t.Name, Value = t });

			foreach (Edge e in new Edge[] { Edge.NONE, Edge.TOWN, Edge.SLOPE, Edge.FOREST })
				_EdgeSelect.Add(
					new SelectionOption<Edge>("select-option") { DisplayedString = e.ToString(), Value = e });

			Add(_Elevation);
			Add(_EdgeSelect);
			Add(_TileBaseSelect);
			Add(_ModeSelect);
		}

		private void PaneChange(object Sender, EventArgs E)
		{
			GuiItem select = ((Select<GuiItem>)Sender).Value.Value;

			_TileBaseSelect.Visible = false;
			_EdgeSelect.Visible = false;
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
				Tile.SetEdge(index, Edge.NONE);
			}
			else if (_ModeSelect.Value.Value == _TileBaseSelect)
			{
				Tile.TileBase = TileBase.CLEAR;
			}
			else if (_ModeSelect.Value.Value == _Elevation)
			{
				Tile.Elevation--;
			}
		}
	}
}
