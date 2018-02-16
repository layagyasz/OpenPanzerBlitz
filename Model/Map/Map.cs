using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class Map : Serializable
	{
		public readonly Environment Environment;
		public readonly Tile[,] Tiles;
		public readonly List<MapRegion> Regions;

		public int Height
		{
			get
			{
				return Tiles.GetLength(1);
			}
		}
		public int Width
		{
			get
			{
				return Tiles.GetLength(0);
			}
		}

		public IEnumerable<Tile> TilesEnumerable
		{
			get
			{
				for (int i = 0; i < Tiles.GetLength(0); ++i)
				{
					for (int j = 0; j < Tiles.GetLength(1); ++j)
					{
						yield return Tiles[i, j];
					}
				}
			}
		}

		public Map(int Width, int Height, Environment Environment, IdGenerator IdGenerator)
		{
			this.Environment = Environment;
			Regions = new List<MapRegion>();
			Tiles = new Tile[Width, Height];

			TileRuleSet ruleSet = Environment == null ? null : Environment.TileRuleSet;
			for (int i = 0; i < Width; ++i)
			{
				for (int j = 0; j < Height; ++j)
				{
					Tiles[i, j] = new Tile(this, new Coordinate(i, j), ruleSet, IdGenerator);
				}
			}
			Ready();
		}

		public Map(SerializationInputStream Stream, TileRuleSet RuleSet, IdGenerator IdGenerator)
		{
			var width = Stream.ReadInt32();
			var height = Stream.ReadInt32();
			Tiles = new Tile[width, height];
			var tiles = Stream.ReadEnumerable(i => new Tile(i, this, RuleSet, IdGenerator)).GetEnumerator();

			for (int i = 0; i < width; ++i)
			{
				for (int j = 0; j < height; ++j)
				{
					tiles.MoveNext();
					Tiles[i, j] = tiles.Current;
				}
			}
			Regions = Stream.ReadEnumerable(i => new MapRegion(Stream, Tiles)).ToList();
			Ready();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Width);
			Stream.Write(Height);
			Stream.Write(TilesEnumerable);
			Stream.Write(Regions);
		}

		internal void CopyTo(Map From, int X, int Y, bool Invert)
		{
			var max = new Coordinate(From.Tiles.GetLength(0), From.Tiles.GetLength(1));

			foreach (MapRegion m in From.Regions)
			{
				var copy = Regions.FirstOrDefault(i => i.Name == m.Name);
				if (copy == null) copy = new MapRegion { Name = m.Name };
				foreach (Tile t in m.Tiles)
				{
					var c = TransformCoordinate(t.Coordinate, new Coordinate(X, Y), max, Invert);
					copy.Add(Tiles[c.X + X, c.Y + Y]);
				}
				Regions.Add(copy);
			}

			for (int i = 0; i < From.Tiles.GetLength(0); ++i)
			{
				for (int j = 0; j < From.Tiles.GetLength(1); ++j)
				{
					var c = TransformCoordinate(new Coordinate(i, j), new Coordinate(X, Y), max, Invert);
					if (c.X < From.Tiles.GetLength(0) && c.X >= 0)
					{
						Tile t = From.Tiles[c.X, c.Y];
						Tiles[X + i, Y + j].Configuration.Merge(new TileConfiguration(
							t.Configuration, Invert));
					}
				}
			}
		}

		Coordinate TransformCoordinate(Coordinate Coordinate, Coordinate Offset, Coordinate Max, bool Invert)
		{
			return Invert ? new Coordinate(
								Max.X - Coordinate.X - 1 - ((Offset.X + Coordinate.Y) % 2 == 0 ? 1 : 0),
								Max.Y - Coordinate.Y - 1)
									: Coordinate;
		}

		internal void Ready()
		{
			for (int i = 0; i < Tiles.GetLength(0); ++i)
			{
				for (int j = 0; j < Tiles.GetLength(1); ++j)
				{
					int xOffset = j % 2 == 0 ? 0 : -1;

					Tile t = Tiles[i, j];
					if (j < Tiles.GetLength(1) - 1)
					{
						if (i + xOffset >= 0) t.SetNeighbor(5, Tiles[i + xOffset, j + 1]);
						if (i < Tiles.GetLength(0) - 1 - xOffset) t.SetNeighbor(4, Tiles[i + 1 + xOffset, j + 1]);
					}
					if (j > 0)
					{
						if (i + xOffset >= 0) t.SetNeighbor(1, Tiles[i + xOffset, j - 1]);
						if (i < Tiles.GetLength(0) - 1 - xOffset) t.SetNeighbor(2, Tiles[i + 1 + xOffset, j - 1]);
					}
					if (i < Tiles.GetLength(0) - 1) t.SetNeighbor(3, Tiles[i + 1, j]);
					if (i > 0) t.SetNeighbor(0, Tiles[i - 1, j]);
				}
			}
			foreach (Tile t in TilesEnumerable) t.FixPaths();
		}
	}
}
