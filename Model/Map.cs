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

		public Map(MapConfiguration MapConfiguration)
		{
			List<List<Tuple<Map, bool>>> boards = MapConfiguration.Boards.Select(i => i.Select(j =>
			{
				FileStream f = new FileStream(j.Item1, FileMode.Open);
				Map map = new Map(new SerializationInputStream(new GZipStream(f, CompressionMode.Decompress)));
				f.Close();
				return new Tuple<Map, bool>(map, j.Item2);
			}).ToList()).ToList();

			int width = boards.Max(i => i.Sum(j => j.Item1.Width) - i.Count + 1);
			int height = boards.Sum(i => i.Max(j => j.Item1.Height)) - MapConfiguration.Boards.Count + 1;

			Tiles = new Tile[width, height];

			int rowY = 0;
			foreach (List<Tuple<Map, bool>> mapRow in boards)
			{
				int rowX = 0;
				int nextRowY = 0;
				foreach (Tuple<Map, bool> map in mapRow)
				{
					CopyTo(Tiles, map.Item1.Tiles, rowX, rowY, map.Item2);
					rowX += map.Item1.Width - 1;
					nextRowY = Math.Max(nextRowY, map.Item1.Height - 1);
				}
				rowY = nextRowY;
			}


			SetupNeighbors();
		}

		public Map(int Width, int Height)
		{
			Tiles = new Tile[Width, Height];
			for (int i = 0; i < Width; ++i)
			{
				for (int j = 0; j < Height; ++j)
				{
					Tiles[i, j] = new Tile(i, j);
					Tiles[i, j].TileBase = TileBase.CLEAR;
				}
			}
			SetupNeighbors();
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
			SetupNeighbors();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Width);
			Stream.Write(Height);
			Stream.Write(TilesEnumerable);
		}

		private void CopyTo(Tile[,] To, Tile[,] From, int X, int Y, bool Invert)
		{
			for (int i = 0; i < From.GetLength(0) && X + i < To.GetLength(0); ++i)
				for (int j = 0; j < From.GetLength(1); ++j)
					if (Invert)
					{
						int x = From.GetLength(0) - i - 1 - ((Y + j) % 2 == 0 ? 1 : 0);
						if (x < From.GetLength(0) && x >= 0)
							To[X + i, Y + j] = new Tile(X + i, Y + j, From[x, From.GetLength(1) - j - 1], Invert);
						else
						{
							To[X + i, Y + j] = new Tile(X + i, Y + j);
							To[X + i, Y + j].TileBase = TileBase.CLEAR;
						}
					}
					else To[X + i, Y + j] = new Tile(X + i, Y + j, From[i, j]);
		}

		private void SetupNeighbors()
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
		}
	}
}
