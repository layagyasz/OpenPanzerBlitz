using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class LineOfFireObjective : Objective
	{
		enum Attribute { FRIENDLY, BREAK_THROUGH, WIDTH, VERTICAL }

		public readonly bool Friendly;
		public readonly bool BreakThrough;
		public readonly byte Width;
		public readonly bool Vertical;

		int _Score;

		public LineOfFireObjective(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Friendly = Parse.DefaultIfNull(attributes[(int)Attribute.FRIENDLY], true);
			BreakThrough = Parse.DefaultIfNull(attributes[(int)Attribute.BREAK_THROUGH], false);
			Width = Parse.DefaultIfNull(attributes[(int)Attribute.WIDTH], (byte)1);
			Vertical = (bool)attributes[(int)Attribute.VERTICAL];
		}

		public int CalculateScore(Army ForArmy, Match Match)
		{
			IEnumerable<Tile> losTiles =
				Match.Armies
					 .Where(i => Friendly == (i.Configuration.Team == ForArmy.Configuration.Team))
					 .SelectMany(i => i.Units)
					 .SelectMany(i => i.GetFieldOfSight(AttackMethod.NORMAL_FIRE))
					 .Select(i => i.Final);

			HashSet<Tile> image;
			HashSet<Tile> negative;

			if (BreakThrough) negative = new HashSet<Tile>(losTiles);
			else negative = new HashSet<Tile>(Match.Map.TilesEnumerable.Except(losTiles));

			for (int i = 0; i < Width - 1; ++i)
			{
				List<Tile> tiles = new List<Tile>();
				foreach (Tile t in negative)
				{
					for (int j = 1; j < 4; ++j)
					{
						Tile frontier = t.NeighborTiles[j];
						if (frontier != null && !negative.Contains(frontier)) tiles.Add(frontier);
					}
				}
				tiles.ForEach(k => negative.Add(k));
			}

			image = new HashSet<Tile>(Match.Map.TilesEnumerable.Except(negative));

			Tile source;
			Tile sink;
			List<Tile> sourceEdge;
			List<Tile> sinkEdge;
			if (Vertical)
			{
				source = new Tile(new Coordinate(Match.Map.Width / 2, -1));
				sink = new Tile(new Coordinate(Match.Map.Width / 2, Match.Map.Height));
				sourceEdge = Match.Map.TilesEnumerable.Where(i => i.OnEdge(Direction.NORTH)).ToList();
				sinkEdge = Match.Map.TilesEnumerable.Where(i => i.OnEdge(Direction.SOUTH)).ToList();
			}
			else
			{
				source = new Tile(new Coordinate(-1, Match.Map.Height / 2));
				sink = new Tile(new Coordinate(Match.Map.Width, Match.Map.Height / 2));
				sourceEdge = Match.Map.TilesEnumerable.Where(i => i.OnEdge(Direction.WEST)).ToList();
				sinkEdge = Match.Map.TilesEnumerable.Where(i => i.OnEdge(Direction.EAST)).ToList();
			}

			image.Add(sink);
			Path<Tile> p = new Path<Tile>(
				source,
				sink,
				i => true,
				(i, j) => image.Contains(j) ? 1 : double.MaxValue,
				(i, j) => i.HeuristicDistanceTo(j),
				i => GetNeighbors(source, sink, sourceEdge, sinkEdge, i),
				(i, j) => i == j);

			_Score = p.Distance < double.MaxValue ? 1 : 0;
			return _Score;
		}

		IEnumerable<Tile> GetNeighbors(Tile Source, Tile Sink, List<Tile> SourceEdge, List<Tile> SinkEdge, Tile Tile)
		{
			if (Tile == Source)
			{
				foreach (Tile t in SourceEdge) yield return t;
			}
			if (SinkEdge.Contains(Tile)) yield return Sink;
			foreach (Tile t in Tile.Neighbors()) yield return t;
		}

		public int GetScore()
		{
			return _Score;
		}
	}
}
