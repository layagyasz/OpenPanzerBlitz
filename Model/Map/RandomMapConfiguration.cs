using System;
using System.Linq;

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

		public Map GenerateMap(IdGenerator IdGenerator)
		{
			Map map = new Map(_Width, _Height, TileRuleSet.SUMMER_STEPPE, IdGenerator);

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
				Bias = .7
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

				for (int i = 0; i < 6; ++i)
				{
					if (t.Configuration.TileBase == TileBase.SLOPE
						|| t.Configuration.TileBase == TileBase.SWAMP
						|| t.Configuration.GetEdge(i) == TileEdge.WATER)
						continue;

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

			map.Ready();
			return map;
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

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(_Width);
			Stream.Write(_Height);
		}
	}
}
