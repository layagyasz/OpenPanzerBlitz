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
		public readonly Tile[,] Tiles;

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

		public Map(int Width, int Height)
		{
			Tiles = new Tile[Width, Height];
			for (int i = 0; i < Width; ++i)
			{
				for (int j = 0; j < Height; ++j)
				{
					Tiles[i, j] = new Tile(new Coordinate(i, j));
					Tiles[i, j].TileBase = TileBase.CLEAR;
				}
			}
			Ready();
		}

		public Map(SerializationInputStream Stream)
		{
			int width = Stream.ReadInt32();
			int height = Stream.ReadInt32();
			Tiles = new Tile[width, height];
			IEnumerator<Tile> tiles = Stream.ReadEnumerable(i => new Tile(i)).GetEnumerator();

			for (int i = 0; i < width; ++i)
			{
				for (int j = 0; j < height; ++j)
				{
					tiles.MoveNext();
					Tiles[i, j] = tiles.Current;
				}
			}
			Ready();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Width);
			Stream.Write(Height);
			Stream.Write(TilesEnumerable);
		}

		internal void CopyTo(Tile[,] From, int X, int Y, bool Invert)
		{
			for (int i = 0; i < From.GetLength(0) && X + i < Tiles.GetLength(0); ++i)
				for (int j = 0; j < From.GetLength(1); ++j)
					if (Invert)
					{
						int x = From.GetLength(0) - i - 1 - ((Y + j) % 2 == 0 ? 1 : 0);
						if (x < From.GetLength(0) && x >= 0)
						{
							Tile newTile = new Tile(
								new Coordinate(X + i, Y + j), From[x, From.GetLength(1) - j - 1], Invert);
							if (Tiles[X + i, Y + j] == null) Tiles[X + i, Y + j] = newTile;
							else Tiles[X + i, Y + j].Merge(newTile);
						}
						else
						{
							Tiles[X + i, Y + j] = new Tile(new Coordinate(X + i, Y + j));
							Tiles[X + i, Y + j].TileBase = TileBase.CLEAR;
						}
					}
					else Tiles[X + i, Y + j] = new Tile(new Coordinate(X + i, Y + j), From[i, j]);
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
