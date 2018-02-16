using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class LineOfFireObjective : Objective
	{
		enum Attribute { FRIENDLY, INCLUDE_FIELD_OF_SIGHT, BREAK_THROUGH, WIDTH, VERTICAL }

		public readonly bool Friendly;
		public readonly bool IncludeFieldOfSight;
		public readonly bool BreakThrough;
		public readonly byte Width;
		public readonly bool Vertical;

		public LineOfFireObjective(bool Friendly, bool IncludeFieldOfSight, bool BreakThrough, byte Width, bool Vertical)
		{
			this.Friendly = Friendly;
			this.IncludeFieldOfSight = IncludeFieldOfSight;
			this.BreakThrough = BreakThrough;
			this.Width = Width;
			this.Vertical = Vertical;
		}

		public LineOfFireObjective(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Friendly = Parse.DefaultIfNull(attributes[(int)Attribute.FRIENDLY], true);
			IncludeFieldOfSight = Parse.DefaultIfNull(attributes[(int)Attribute.INCLUDE_FIELD_OF_SIGHT], true);
			BreakThrough = Parse.DefaultIfNull(attributes[(int)Attribute.BREAK_THROUGH], false);
			Width = Parse.DefaultIfNull(attributes[(int)Attribute.WIDTH], (byte)1);
			Vertical = (bool)attributes[(int)Attribute.VERTICAL];
		}

		public LineOfFireObjective(SerializationInputStream Stream)
			: this(
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadBoolean(),
				Stream.ReadByte(),
				Stream.ReadBoolean())
		{ }

		public override void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Friendly);
			Stream.Write(IncludeFieldOfSight);
			Stream.Write(BreakThrough);
			Stream.Write(Width);
			Stream.Write(Vertical);
		}

		public override bool CanStopEarly()
		{
			return false;
		}

		public override int CalculateScore(Army ForArmy, Match Match, Dictionary<Objective, int> Cache)
		{
			var units = Match.Armies
								 .Where(i => Friendly == (i.Configuration.Team == ForArmy.Configuration.Team))
										   .SelectMany(i => i.Units).Where(i => i.Position != null);
			var losTiles = IncludeFieldOfSight
				? units.SelectMany(i => i.GetFieldOfSight(AttackMethod.NORMAL_FIRE)).Select(i => i.Item1.Final)
					   : units.Select(i => i.Position);

			HashSet<Tile> image;
			HashSet<Tile> negative;

			if (BreakThrough) negative = new HashSet<Tile>(losTiles);
			else negative = new HashSet<Tile>(Match.Map.TilesEnumerable.Except(losTiles));

			for (int i = 0; i < Width - 1; ++i)
			{
				var tiles = new List<Tile>();
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
				source = new Tile(Match.Map, new Coordinate(Match.Map.Width / 2, -1), null, new IdGenerator());
				sink = new Tile(
					Match.Map, new Coordinate(Match.Map.Width / 2, Match.Map.Height), null, new IdGenerator());
				sourceEdge = Match.Map.TilesEnumerable.Where(i => i.OnEdge(Direction.NORTH)).ToList();
				sinkEdge = Match.Map.TilesEnumerable.Where(i => i.OnEdge(Direction.SOUTH)).ToList();
			}
			else
			{
				source = new Tile(Match.Map, new Coordinate(-1, Match.Map.Height / 2), null, new IdGenerator());
				sink = new Tile(
					Match.Map, new Coordinate(Match.Map.Width, Match.Map.Height / 2), null, new IdGenerator());
				sourceEdge = Match.Map.TilesEnumerable.Where(i => i.OnEdge(Direction.WEST)).ToList();
				sinkEdge = Match.Map.TilesEnumerable.Where(i => i.OnEdge(Direction.EAST)).ToList();
			}

			image.Add(sink);
			var p = new Path<Tile>(
				source,
				sink,
				i => true,
				(i, j) => image.Contains(j) ? 1 : double.MaxValue,
				(i, j) => i.HeuristicDistanceTo(j),
				i => GetNeighbors(source, sink, sourceEdge, sinkEdge, i),
				(i, j) => i == j);

			return p.Distance < double.MaxValue ? 1 : 0;
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

		public override IEnumerable<Tile> GetTiles(Map Map)
		{
			return Enumerable.Empty<Tile>();
		}
	}
}
