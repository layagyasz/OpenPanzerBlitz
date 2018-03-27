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
		public EventHandler OnAddMapRegion;
		public EventHandler<ValuedEventArgs<MapRegion>> OnDeleteMapRegion;
		public EventHandler<ValuedEventArgs<MapRegion>> OnMapRegionSelected;
		public EventHandler OnElevationTransitionChanged;

		readonly Select<Pod> _ModeSelect = new Select<Pod>("select");

		readonly Container<Pod> _TileBasePage = new Container<Pod>();
		readonly Container<Pod> _EdgePage = new Container<Pod>();
		readonly Container<Pod> _PathOverlayPage = new Container<Pod>();
		readonly Container<Pod> _ElevationPage = new Container<Pod>();
		readonly Container<Pod> _MapRegionPage = new Container<Pod>();

		Select<TileBase> _TileBaseSelect = new Select<TileBase>("select");
		Select<TileEdge> _EdgeSelect = new Select<TileEdge>("select");
		Select<TilePathOverlay> _PathOverlaySelect = new Select<TilePathOverlay>("select");
		Button _Elevation = new Button("select-option") { DisplayedString = "Left/Right Click" };
		Select<MapRegion> _MapRegionSelect = new Select<MapRegion>("select");
		Button _MapRegionDeleteButton = new Button("small-button") { DisplayedString = "Delete" };
		Button _MapRegionAddButton = new Button("small-button") { DisplayedString = "Add" };

		public EditPane(Map Map)
			: base("edit-pane")
		{
			_ModeSelect.OnChange += PaneChange;
			_ModeSelect.Add(
				new SelectionOption<Pod>("select-option")
				{
					DisplayedString = "Base",
					Value = _TileBasePage
				});
			_ModeSelect.Add(
				new SelectionOption<Pod>("select-option")
				{
					DisplayedString = "Overlay",
					Value = _PathOverlayPage
				});
			_ModeSelect.Add(
				new SelectionOption<Pod>("select-option") { DisplayedString = "Edge", Value = _EdgePage });
			_ModeSelect.Add(
				new SelectionOption<Pod>("select-option")
				{
					DisplayedString = "Elevation",
					Value = _ElevationPage
				});
			_ModeSelect.Add(
				new SelectionOption<Pod>("select-option")
				{
					DisplayedString = "Map Region",
					Value = _MapRegionPage
				});

			var header = new Button("edit-header-1") { DisplayedString = "Edit" };
			_ModeSelect.Position = new Vector2f(0, header.Size.Y);
			_TileBaseSelect.Position = new Vector2f(0, _ModeSelect.Position.Y + _ModeSelect.Size.Y + 2);
			_PathOverlaySelect.Position = new Vector2f(0, _ModeSelect.Position.Y + _ModeSelect.Size.Y + 2);
			_EdgeSelect.Position = new Vector2f(0, _ModeSelect.Position.Y + _ModeSelect.Size.Y + 2);
			_Elevation.Position = new Vector2f(0, _ModeSelect.Position.Y + _ModeSelect.Size.Y + 2);
			_MapRegionSelect.Position = new Vector2f(0, _ModeSelect.Position.Y + _ModeSelect.Size.Y + 2);

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

			UpdateFromMap(Map);

			_MapRegionAddButton.Position = new Vector2f(
				Size.X - _MapRegionAddButton.Size.X - 32, Size.Y - _MapRegionAddButton.Size.Y - 32);
			_MapRegionAddButton.OnClick += HandleAddMapRegion;
			_MapRegionDeleteButton.Position = _MapRegionAddButton.Position
				- new Vector2f(_MapRegionDeleteButton.Size.X + 8, 0);
			_MapRegionDeleteButton.OnClick += HandleDeleteMapRegion;
			_MapRegionSelect.OnChange += HandleSelectMapRegion;

			_ElevationPage.Add(_Elevation);
			_PathOverlayPage.Add(_PathOverlaySelect);
			_EdgePage.Add(_EdgeSelect);
			_TileBasePage.Add(_TileBaseSelect);
			_MapRegionPage.Add(_MapRegionAddButton);
			_MapRegionPage.Add(_MapRegionDeleteButton);
			_MapRegionPage.Add(_MapRegionSelect);

			Add(header);
			Add(_ElevationPage);
			Add(_PathOverlayPage);
			Add(_EdgePage);
			Add(_TileBasePage);
			Add(_MapRegionPage);
			Add(_ModeSelect);
		}

		public void UpdateFromMap(Map Map)
		{
			_MapRegionSelect.Clear();
			foreach (MapRegion r in Map.Regions)
			{
				_MapRegionSelect.Add(new SelectionOption<MapRegion>("select-option")
				{
					DisplayedString = r.Name,
					Value = r
				});
			}
		}

		void PaneChange(object Sender, EventArgs E)
		{
			var select = (Container<Pod>)_ModeSelect.Value.Value;

			_TileBasePage.Visible = false;
			_EdgePage.Visible = false;
			_PathOverlayPage.Visible = false;
			_ElevationPage.Visible = false;
			_MapRegionPage.Visible = false;

			select.Visible = true;
		}

		void HandleAddMapRegion(object Sender, EventArgs E)
		{
			if (OnAddMapRegion != null) OnAddMapRegion(this, EventArgs.Empty);
		}

		void HandleDeleteMapRegion(object Sender, EventArgs E)
		{
			if (OnDeleteMapRegion != null)
				OnDeleteMapRegion(this, new ValuedEventArgs<MapRegion>(_MapRegionSelect.Value.Value));
		}

		void HandleSelectMapRegion(object Sender, ValuedEventArgs<StandardItem<MapRegion>> E)
		{
			if (OnMapRegionSelected != null) OnMapRegionSelected(this, new ValuedEventArgs<MapRegion>(E.Value.Value));
		}

		public void EditTile(Tile Tile, Vector2f Point)
		{
			if (_ModeSelect.Value.Value == _EdgePage)
			{
				var index = Enumerable.Range(0, 6).ArgMax(i => -Tile.Bounds[i].DistanceSquared(Point));
				Tile.SetEdge(index, _EdgeSelect.Value.Value);
			}
			else if (_ModeSelect.Value.Value == _TileBasePage)
			{
				Tile.Configuration.SetTileBase(_TileBaseSelect.Value.Value);
			}
			else if (_ModeSelect.Value.Value == _PathOverlayPage)
			{
				var index = Enumerable.Range(0, 6).ArgMax(i => -Tile.Bounds[i].DistanceSquared(Point));
				Tile.SetPathOverlay(index, _PathOverlaySelect.Value.Value);
			}
			else if (_ModeSelect.Value.Value == _ElevationPage)
			{
				Tile.Configuration.SetElevation((byte)(Tile.Configuration.Elevation + 1));
			}
			else if (_ModeSelect.Value.Value == _MapRegionPage)
			{
				if (_MapRegionSelect.Value != null) _MapRegionSelect.Value.Value.Add(Tile);
			}
		}

		public void ShiftEditTile(Tile Tile, Vector2f Point)
		{
			if (_ModeSelect.Value.Value == _EdgePage)
			{
				for (int i = 0; i < 6; ++i) Tile.SetEdge(i, _EdgeSelect.Value.Value);
			}
			else if (_ModeSelect.Value.Value == _ElevationPage)
			{
				Tile.Configuration.SetElevationTransition(!Tile.Configuration.ElevationTransition);
				if (OnElevationTransitionChanged != null) OnElevationTransitionChanged(this, EventArgs.Empty);
			}
		}

		public void RightEditTile(Tile Tile, Vector2f Point)
		{
			if (_ModeSelect.Value.Value == _EdgePage)
			{
				var index = Enumerable.Range(0, 6).ArgMax(i => -Tile.Bounds[i].DistanceSquared(Point));
				Tile.SetEdge(index, TileEdge.NONE);
			}
			else if (_ModeSelect.Value.Value == _TileBasePage)
			{
				Tile.Configuration.SetTileBase(TileBase.CLEAR);
			}
			else if (_ModeSelect.Value.Value == _PathOverlayPage)
			{
				var index = Enumerable.Range(0, 6).ArgMax(i => -Tile.Bounds[i].DistanceSquared(Point));
				Tile.SetPathOverlay(index, TilePathOverlay.NONE);
			}
			else if (_ModeSelect.Value.Value == _ElevationPage)
			{
				Tile.Configuration.SetElevation((byte)(Tile.Configuration.Elevation - 1));
			}
			else if (_ModeSelect.Value.Value == _MapRegionPage)
			{
				if (_MapRegionSelect.Value != null) _MapRegionSelect.Value.Value.Remove(Tile);
			}
		}

		public void ShiftRightEditTile(Tile Tile, Vector2f Point)
		{
			if (_ModeSelect.Value.Value == _EdgePage)
			{
				for (int i = 0; i < 6; ++i) Tile.SetEdge(i, TileEdge.NONE);
			}
		}
	}
}
