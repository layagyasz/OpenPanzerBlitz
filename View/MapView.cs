using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class MapView : Pod
	{
		private Map _Map;
		public readonly TileView[,] Tiles;

		public Map Map
		{
			get
			{
				return _Map;
			}
		}

		public IEnumerable<TileView> TilesEnumerable
		{
			get
			{
				for (int i = 0; i < _Map.Tiles.GetLength(0); ++i)
				{
					for (int j = 0; j < _Map.Tiles.GetLength(1); ++j)
					{
						yield return Tiles[i, j];
					}
				}
			}
		}

		public MapView(Map Map, TileRenderer TileRenderer)
		{
			_Map = Map;
			Tiles = new TileView[_Map.Tiles.GetLength(0), _Map.Tiles.GetLength(1)];
			for (int i = 0; i < _Map.Tiles.GetLength(0); ++i)
			{
				for (int j = 0; j < _Map.Tiles.GetLength(1); ++j)
				{
					Tiles[i, j] = new TileView(_Map.Tiles[i, j], TileRenderer);
				}
			}
		}

		public void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			foreach (TileView t in TilesEnumerable) t.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public void Draw(RenderTarget Target, Transform Transform)
		{
			foreach (TileView t in TilesEnumerable) t.Draw(Target, Transform);
		}
	}
}
