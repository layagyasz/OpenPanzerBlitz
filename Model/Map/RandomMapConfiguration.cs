using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Serialization;

using Cence;

using SFML.Window;

namespace PanzerBlitz
{
	public class RandomMapConfiguration : MapConfiguration
	{
		int _Width;
		int _Height;
		Random _Random;

		public RandomMapConfiguration(int Width, int Height)
		{
			_Width = Width;
			_Height = Height;
			_Random = new Random();
		}

		public RandomMapConfiguration(SerializationInputStream Stream)
			: this(Stream.ReadInt32(), Stream.ReadInt32()) { }

		public Map GenerateMap(Environment Environment, IdGenerator IdGenerator)
		{
			Map map = new Map(_Width, _Height, Environment, IdGenerator);

			LatticeNoiseGenerator thresholdNoise = new LatticeNoiseGenerator(_Random, new LatticeNoiseSettings()
			{
				Frequency = Constant.Create(.1),
				Factor = .1,
				Bias = .63
			});
			LatticeNoiseGenerator waterThresholdNoise = new LatticeNoiseGenerator(_Random, new LatticeNoiseSettings()
			{
				Frequency = Constant.Create(.1),
				Factor = .1,
				Bias = .3
			});
			LatticeNoiseGenerator noise = MakeNoiseGenerator(.1, .2, .5);

			LatticeNoiseGenerator swampThresholdNoise = new LatticeNoiseGenerator(_Random, new LatticeNoiseSettings()
			{
				Frequency = Constant.Create(.1),
				Factor = .3,
				Bias = .35
			});
			LatticeNoiseGenerator swampNoise = MakeNoiseGenerator(.075, .175, .5);

			LatticeNoiseGenerator forestThresholdNoise = new LatticeNoiseGenerator(_Random, new LatticeNoiseSettings()
			{
				Frequency = Constant.Create(.15),
				Factor = .1,
				Bias = .6
			});
			LatticeNoiseGenerator forestNoise = MakeNoiseGenerator(.25, .5, .5);

			LatticeNoiseGenerator townThresholdNoise = new LatticeNoiseGenerator(_Random, new LatticeNoiseSettings()
			{
				Frequency = Constant.Create(.1),
				Factor = .2,
				Bias = .72
			});
			LatticeNoiseGenerator townNoise = MakeNoiseGenerator(.25, .5, .5);

			foreach (Tile t in map.TilesEnumerable)
			{
				double elevation = noise.Generate(t.Center.X, t.Center.Y);
				if (elevation > thresholdNoise.Generate(t.Center.X, t.Center.Y) && t.OnEdge(Direction.NONE))
					t.Configuration.SetElevation(1);
				else if (elevation < waterThresholdNoise.Generate(t.Center.X, t.Center.Y))
				{
					for (int i = 0; i < 6; ++i) t.SetEdge(i, TileEdge.WATER);
				}
				if (t.Configuration.Elevation == 0 && swampNoise.Generate(t.Center.X, t.Center.Y)
					< swampThresholdNoise.Generate(t.Center.X, t.Center.Y))
					t.Configuration.SetTileBase(TileBase.SWAMP);
			}
			foreach (Tile t in map.TilesEnumerable)
			{
				if (t.NeighborTiles.Any(i => i != null && i.Configuration.Elevation > t.Configuration.Elevation))
					t.Configuration.SetTileBase(TileBase.SLOPE);
			}
			foreach (Tile t in map.TilesEnumerable)
			{
				if (t.Configuration.TileBase == TileBase.SLOPE
					|| t.Configuration.TileBase == TileBase.SWAMP)
					continue;

				for (int i = 0; i < 6; ++i)
				{
					if (t.Configuration.TileBase == TileBase.SLOPE || t.Configuration.GetEdge(i) == TileEdge.WATER)
						continue;

					Tile neighbor = t.NeighborTiles[i];
					if (neighbor != null
						&& neighbor.Configuration.TileBase != TileBase.SLOPE
						&& neighbor.Configuration.TileBase != TileBase.SWAMP)
					{
						Vector2f v = .5f * (t.Bounds[i].Point + t.Bounds[i].End);
						if (forestNoise.Generate(v.X, v.Y) > forestThresholdNoise.Generate(v.X, v.Y))
							t.SetEdge(i, TileEdge.FOREST);
						if (townNoise.Generate(t.Bounds[i].Point.X, t.Bounds[i].Point.Y)
							> townThresholdNoise.Generate(t.Bounds[i].Point.X, t.Bounds[i].Point.Y)
						   || townNoise.Generate(t.Bounds[i].End.X, t.Bounds[i].End.Y)
							> townThresholdNoise.Generate(t.Bounds[i].End.X, t.Bounds[i].End.Y))
							t.SetEdge(i, TileEdge.TOWN);
					}
				}
			}

			// Rivers
			HashSet<Tile> riverNodes = new HashSet<Tile>();
			for (int i = 0; i < Math.Max(1, _Width * _Height / 160); ++i)
			{
				Tile t = GetRandomTile(map);
				if (!IsElevated(t)) riverNodes.Add(t);
			}
			for (int i = 0; i < Math.Max(2, (_Width + _Height - 2) / 8); ++i)
			{
				Tile t = GetRandomEdgeTile(map);
				if (!IsElevated(t))
				{
					EdgePathOverlay(t, TilePathOverlay.STREAM);
					riverNodes.Add(t);
				}
			}
			MinimalSpanning<Tile> mst =
				new MinimalSpanning<Tile>(riverNodes, i => riverNodes, (i, j) => i.HeuristicDistanceTo(j));
			List<Tuple<Tile, Tile>> edges = mst.GetEdges().ToList();
			for (int i = 0; i < edges.Count / 4; ++i)
				edges.RemoveAt(_Random.Next(0, edges.Count));
			foreach (Tuple<Tile, Tile> edge in edges)
				MakePath(edge.Item1, edge.Item2, TilePathOverlay.STREAM, RiverDistanceFunction);

			// Roads and Towns
			Partitioning<Tile> towns = new Partitioning<Tile>(
				map.TilesEnumerable, (i, j) => i.GetEdge(j) == TileEdge.TOWN);
			HashSet<Tile> roadNodes = new HashSet<Tile>();
			foreach (ISet<Tile> town in towns.GetPartitions())
			{
				map.Regions.Add(new MapRegion("Town " + IdGenerator.GenerateId().ToString(), town));
				List<Tile> tiles = town.ToList();
				for (int i = 0; i < Math.Max(1, tiles.Count / 4); ++i)
					roadNodes.Add(tiles[_Random.Next(0, tiles.Count)]);
			}
			for (int i = 0; i < Math.Max(1, _Width * _Height / 160); ++i)
				roadNodes.Add(GetRandomTile(map));
			for (int i = 0; i < Math.Max(2, (_Width + _Height - 2) / 8); ++i)
			{
				Tile t = GetRandomEdgeTile(map);
				EdgePathOverlay(t, TilePathOverlay.ROAD);
				roadNodes.Add(t);
			}

			mst = new MinimalSpanning<Tile>(roadNodes, i => roadNodes, (i, j) => i.HeuristicDistanceTo(j));
			edges = mst.GetEdges().ToList();
			List<Tile> nodes = roadNodes.ToList();
			for (int i = 0; i < edges.Count / 4; ++i)
			{
				edges.Add(
					new Tuple<Tile, Tile>(nodes[_Random.Next(0, nodes.Count)], nodes[_Random.Next(0, nodes.Count)]));
			}
			double m = _Random.NextDouble() * 10;
			foreach (Tuple<Tile, Tile> edge in edges)
				MakePath(edge.Item1, edge.Item2, TilePathOverlay.ROAD, (i, j) => RoadDistanceFunction(i, j, m));

			map.Ready();
			return map;
		}

		public StaticMapConfiguration MakeStaticMap()
		{
			return new StaticMapConfiguration(GenerateMap(null, new IdGenerator()));
		}

		LatticeNoiseGenerator MakeNoiseGenerator(double MinFrequency, double MaxFrequency, double Persistence)
		{
			LatticeNoiseSettings frequencySettings = new LatticeNoiseSettings()
			{
				Frequency = Constant.Create(.1)
			};
			LatticeNoiseGenerator frequencyGenerator = new LatticeNoiseGenerator(_Random, frequencySettings);
			LatticeNoiseSettings settings = new LatticeNoiseSettings()
			{
				Frequency = (i, j) => MinFrequency + (MaxFrequency - MinFrequency) * frequencyGenerator.Generate(i, j),
				Persistence = Constant.Create(Persistence)
			};
			return new LatticeNoiseGenerator(_Random, settings);
		}

		void MakePath(Tile Start, Tile End, TilePathOverlay Path, Func<Tile, Tile, double> DistanceFunction)
		{
			Path<Tile> path = new Path<Tile>(
				Start,
				End,
				i => true,
				DistanceFunction,
				(i, j) => i.HeuristicDistanceTo(j),
				i => i.Neighbors(),
				(i, j) => i == j);
			for (int i = 0; i < path.Count - 1; ++i)
				path[i].SetPathOverlay(Array.IndexOf(path[i].NeighborTiles, path[i + 1]), Path);
		}

		Tile GetRandomTile(Map Map)
		{
			return Map.Tiles[_Random.Next(0, _Width), _Random.Next(0, _Height)];
		}

		Tile GetRandomEdgeTile(Map Map)
		{
			bool xEdge = _Random.Next(0, 2) == 0;
			bool yEdge = _Random.Next(0, 2) == 0;
			int x = 0;
			int y = 0;
			if (xEdge)
			{
				x = _Random.Next(0, _Width);
				y = yEdge ? 0 : _Height - 1;
			}
			else
			{
				x = yEdge ? 0 : _Width - 1;
				y = _Random.Next(0, _Height);
			}
			return Map.Tiles[x, y];
		}

		void EdgePathOverlay(Tile Tile, TilePathOverlay Path)
		{
			if (Tile.OnEdge(Direction.NORTH)) Tile.SetPathOverlay(2, Path);
			else if (Tile.OnEdge(Direction.SOUTH)) Tile.SetPathOverlay(4, Path);
			else if (Tile.OnEdge(Direction.WEST)) Tile.SetPathOverlay(0, Path);
			else if (Tile.OnEdge(Direction.EAST)) Tile.SetPathOverlay(3, Path);
		}

		double RoadDistanceFunction(Tile a, Tile b, double TerrainMultiplier)
		{
			if (a.GetPathOverlay(b) == TilePathOverlay.ROAD) return 0;
			if (a.GetPathOverlay(b) != TilePathOverlay.NONE) return float.MaxValue;
			if (a.GetEdge(b) == TileEdge.WATER) return 40;
			if (b.Configuration.TileBase == TileBase.SWAMP) return 12;
			if (b.Configuration.Edges.Count(i => i == TileEdge.TOWN) > 0) return 1;
			if (b.Configuration.Edges.Count(i => i == TileEdge.SLOPE) > 0) return 6 * TerrainMultiplier;
			if (b.Configuration.TileBase == TileBase.SLOPE) return 6 * TerrainMultiplier;
			if (b.Configuration.PathOverlays.Count(i => i == TilePathOverlay.STREAM) > 0) return 6 * TerrainMultiplier;
			if (b.Configuration.Edges.Count(i => i == TileEdge.FOREST) > 0) return 3;

			return 6 * _Random.NextDouble() + 2;
		}

		bool IsElevated(Tile Tile)
		{
			return Tile.Configuration.Elevation > 0
					   || Tile.Configuration.TileBase == TileBase.SLOPE
					   || Tile.Configuration.Edges.Count(i => i == TileEdge.SLOPE) > 0;
		}

		double RiverDistanceFunction(Tile a, Tile b)
		{
			if (a.GetPathOverlay(b) == TilePathOverlay.STREAM) return 0;
			if (IsElevated(b)) return float.MaxValue;
			if (b.Configuration.Edges.Count(i => i == TileEdge.FOREST) > 0) return .5;

			return 6 * _Random.NextDouble() + 2;
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(_Width);
			Stream.Write(_Height);
		}
	}
}
