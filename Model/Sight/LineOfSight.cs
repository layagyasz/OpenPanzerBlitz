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

			// We calculate lines of sight to and from to make sure sighting is symmetrical.
			var losA = FindLOS(From.Map, From.HexCoordinate, To.HexCoordinate, false);
			TileComponentRules[] crossedEdgesA = new TileComponentRules[losA.Length - 1];

			for (int i = 0; i < losA.Length - 1; ++i)
			{
				crossedEdgesA[i] = losA[i].GetEdgeRules(losA[i + 1]);
			}

			var losAVerify = Verify(losA, crossedEdgesA);
			if (losAVerify != NoLineOfSightReason.NONE)
			{
				_Validated = losAVerify;
				_LineOfSight = losA;
				_CrossedEdges = crossedEdgesA;
			}
			else
			{
				var losB = FindLOS(From.Map, From.HexCoordinate, To.HexCoordinate, true);
				TileComponentRules[] crossedEdgesB = new TileComponentRules[losB.Length - 1];

				for (int i = 0; i < losB.Length - 1; ++i)
				{
					crossedEdgesB[i] = losB[i].GetEdgeRules(losB[i + 1]);
				}

				var losBVerify = Verify(losB, crossedEdgesB);
				if (losBVerify != NoLineOfSightReason.NONE)
				{
					_Validated = losBVerify;
					_LineOfSight = losB;
					_CrossedEdges = crossedEdgesB;
				}
				else
				{
					_Validated = losAVerify;
					_LineOfSight = losA;
					_CrossedEdges = crossedEdgesA;
				}
			}
		}

		public NoLineOfSightReason Validate()
		{
			return _Validated;
		}

		static Tile[] FindLOS(Map Map, HexCoordinate From, HexCoordinate To, bool Dither)
		{
			int count = From.Distance(To);
			Tile[] tiles = new Tile[count + 1];
			double step = 1.0 / count;
			for (int i = 0; i < count + 1; ++i)
			{
				Coordinate c = HexCoordinate.Interpolate(From, To, step * i, Dither ? -.01 : 0).ToCoordinate();
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
