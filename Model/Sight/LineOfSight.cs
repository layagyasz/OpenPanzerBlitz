using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class LineOfSight
	{
		Tile[] _LineOfSight;
		TileComponentRules[] _CrossedEdges;

		NoLineOfSightReason _Validated;

		public IEnumerable<Tile> Tiles
		{
			get
			{
				return _LineOfSight;
			}
		}
		public Tile Initial
		{
			get
			{
				return _LineOfSight[0];
			}
		}
		public Tile Final
		{
			get
			{
				return _LineOfSight[_LineOfSight.Length - 1];
			}
		}
		public int Range
		{
			get
			{
				return _LineOfSight.Length - 1;
			}
		}

		public LineOfSight(Tile From, Tile To)
		{
			if (From == To)
			{
				_LineOfSight = new Tile[] { From };
				_CrossedEdges = new TileComponentRules[0];
				_Validated = NoLineOfSightReason.NONE;
				return;
			}

			FindLOS(From, To, false);
			if (_Validated == NoLineOfSightReason.NONE) FindLOS(From, To, true);
			if (_LineOfSight == null) _Validated = NoLineOfSightReason.TERRAIN;
		}

		public NoLineOfSightReason Validate()
		{
			return _Validated;
		}

		void FindLOS(Tile From, Tile To, bool Dither)
		{
			var los = FindLOS(From.Map, From.HexCoordinate, To.HexCoordinate, Dither);
			if (los == null) return;

			_LineOfSight = los;
			_CrossedEdges = new TileComponentRules[_LineOfSight.Length - 1];
			for (int i = 0; i < _LineOfSight.Length - 1; ++i)
			{
				_CrossedEdges[i] = _LineOfSight[i].GetEdgeRules(_LineOfSight[i + 1]);
			}
			_Validated = Verify(_LineOfSight, _CrossedEdges);
		}

		static Tile[] FindLOS(Map Map, HexCoordinate From, HexCoordinate To, bool Dither)
		{
			var count = From.Distance(To);
			var tiles = new Tile[count + 1];
			var step = 1.0 / count;
			for (int i = 0; i < count + 1; ++i)
			{
				Coordinate c = HexCoordinate.Interpolate(From, To, step * i, Dither ? -.01 : 0).ToCoordinate();
				if (c.X < 0 || c.X >= Map.Tiles.GetLength(0) || c.Y < 0 || c.Y >= Map.Tiles.GetLength(1))
					return null;
				tiles[i] = Map.Tiles[c.X, c.Y];
			}
			return tiles;
		}

		static NoLineOfSightReason Verify(Tile[] LineOfSight, TileComponentRules[] CrossedEdges)
		{
			// Always LOS if adjacent.
			if (CrossedEdges.Length <= 1) return NoLineOfSightReason.NONE;

			// Sort LOS so lower tile is first.
			Tile[] los = null;
			TileComponentRules[] edges = null;
			if (LineOfSight[0].Rules.SubTieredElevation
				> LineOfSight[LineOfSight.Length - 1].Rules.SubTieredElevation)
			{
				los = LineOfSight.Reverse().ToArray();
				edges = CrossedEdges.Reverse().ToArray();
			}
			else
			{
				los = LineOfSight.ToArray();
				edges = CrossedEdges.ToArray();
			}

			// Check for blocks.
			if (ElevationBlocks(los)) return NoLineOfSightReason.TERRAIN;
			if (DepressionBlocks(los)) return NoLineOfSightReason.TERRAIN;
			if (TerrainBlocks(los, edges)) return NoLineOfSightReason.TERRAIN;
			return NoLineOfSightReason.NONE;
		}

		static bool TerrainBlocks(Tile[] LineOfSight, TileComponentRules[] Edges)
		{
			if (LineOfSight[0].Configuration.Elevation == LineOfSight[LineOfSight.Length - 1].Configuration.Elevation)
			{
				for (int i = 0; i < LineOfSight.Length - 1; ++i)
				{
					if (LineOfSight[0].Configuration.Elevation <= LineOfSight[i + 1].Configuration.Elevation)
					{
						if (i < LineOfSight.Length - 2 && LineOfSight[i + 1].GetBaseRules().BlocksLineOfSight)
							return true;
						if (Edges[i] != null
							&& (Edges[i].BlocksLineOfSight || Edges[i].HasAttribute(TerrainAttribute.SLOPED)))
							return true;
					}
				}
			}
			else
			{
				if (Edges[0] != null && Edges[0].BlocksLineOfSight) return true;
				for (int i = 0; i < LineOfSight.Length - 1; ++i)
				{
					if (LineOfSight[i + 1].Configuration.Elevation > LineOfSight[0].Configuration.Elevation)
					{
						if (i < LineOfSight.Length - 2 && LineOfSight[i + 1].GetBaseRules().BlocksLineOfSight)
							return true;
						if (Edges[i] != null
							&& (Edges[i].BlocksLineOfSight || Edges[i].HasAttribute(TerrainAttribute.SLOPED)))
							return true;
					}
				}
			}
			return false;
		}

		static bool DepressionBlocks(Tile[] LineOfSight)
		{
			if (LineOfSight[0].Rules.Depressed)
				return LineOfSight[LineOfSight.Length - 1]
					.Rules.SubTieredElevation <= LineOfSight[0].Rules.SubTieredElevation
					|| LineOfSight[LineOfSight.Length - 1].Rules.Depressed;
			return LineOfSight[LineOfSight.Length - 1].Rules.Depressed;
		}

		static bool ElevationBlocks(Tile[] LineOfSight)
		{
			if (LineOfSight[0].Rules.SubTieredElevation
				== LineOfSight[LineOfSight.Length - 1].Rules.SubTieredElevation)
				return CheckElevation(LineOfSight, LineOfSight.Length);

			if (LineOfSight.Length == 3
				&& LineOfSight[0].Configuration.Elevation == LineOfSight[1].Configuration.Elevation) return false;
			if (CheckElevation(LineOfSight, Math.Min(LineOfSight.Length - 1, (LineOfSight.Length - 1) / 2 + 1)))
				return true;
			return LineOfSight.Any(
				i => i.Rules.SubTieredElevation > LineOfSight[0].Rules.SubTieredElevation
				&& i.Rules.SubTieredElevation > LineOfSight[LineOfSight.Length - 1].Rules.SubTieredElevation);
		}

		static bool CheckElevation(Tile[] LineOfSight, int End)
		{
			int elevation = LineOfSight[0].Rules.SubTieredElevation;
			for (int i = 0; i < End; ++i)
			{
				if (LineOfSight[i].Configuration.ElevationTransition)
				{
					if (i < LineOfSight.Length - 1 && LineOfSight[i].Rules.SubTieredElevation == elevation)
						elevation = LineOfSight[i + 1].Rules.SubTieredElevation;
					else elevation = LineOfSight[i].Rules.SubTieredElevation;
				}
				if (LineOfSight[i].Rules.SubTieredElevation > elevation) return true;
			}
			return false;
		}
	}
}
