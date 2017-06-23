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

		Select<TileConfiguration> _TileConfigurationSelect = new Select<TileConfiguration>("select");
		Select<Edge> _EdgeSelect = new Select<Edge>("select");

		public EditPane()
			: base("edit-pane")
		{
			_ModeSelect.OnChange += PaneChange;
			_ModeSelect.Add(
				new SelectionOption<GuiItem>("select-option")
				{
					DisplayedString = "Base Tile",
					Value = _TileConfigurationSelect
				});
			// _ModeSelect.Add(new SelectionOption<object>("select-option") { DisplayedString = "Overlay" });
			_ModeSelect.Add(
				new SelectionOption<GuiItem>("select-option") { DisplayedString = "Edge", Value = _EdgeSelect });

			_TileConfigurationSelect.Position = new Vector2f(0, _ModeSelect.Size.Y + 2);
			_EdgeSelect.Position = new Vector2f(0, _ModeSelect.Size.Y + 2);

			foreach (TileConfiguration t in
					 new TileConfiguration[]
			{
				TileConfiguration.CLEAR,
				TileConfiguration.SLOPE,
				TileConfiguration.SWAMP,
				TileConfiguration.WATER
			})
				_TileConfigurationSelect.Add(
					new SelectionOption<TileConfiguration>("select-option") { DisplayedString = t.Name, Value = t });

			foreach (Edge e in new Edge[] { Edge.NONE, Edge.TOWN, Edge.SLOPE, Edge.FOREST })
				_EdgeSelect.Add(
					new SelectionOption<Edge>("select-option") { DisplayedString = e.ToString(), Value = e });

			Add(_EdgeSelect);
			Add(_TileConfigurationSelect);
			Add(_ModeSelect);
		}

		private void PaneChange(object Sender, EventArgs E)
		{
			GuiItem select = ((Select<GuiItem>)Sender).Value.Value;

			_TileConfigurationSelect.Visible = false;
			_EdgeSelect.Visible = false;

			select.Visible = true;
		}

		public void EditTile(Tile Tile, Vector2f Point)
		{
			if (_ModeSelect.Value.Value == _EdgeSelect)
			{
				int index = Enumerable.Range(0, 6).ArgMax(i => -Tile.Bounds[i].DistanceSquared(Point));
				Tile.SetEdge(index, _EdgeSelect.Value.Value);
			}
			else if (_ModeSelect.Value.Value == _TileConfigurationSelect)
			{
				Tile.Reconfigure(_TileConfigurationSelect.Value.Value);
			}
		}
	}
}
