using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Planar;

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
			// We calculate lines of sight to and from to make sure sighting is symmetrical.
			Segment s = new Segment(From.Center, To.Center);
			Tile[] losA = new Path<Tile>(
				From,
				To,
				i => true,
				(i, j) => 1,
				(i, j) => s.DistanceSquared(i.Center),
				i => i.Neighbors(),
				(i, j) => i == j).Nodes.ToArray();
			TileComponentRules[] crossedEdgesA = new TileComponentRules[losA.Length - 1];

			for (int i = 0; i < losA.Length - 1; ++i)
			{
				crossedEdgesA[i] = losA[i].GetEdgeRules(losA[i + 1]);
			}

			NoLineOfSightReason losAVerify = Verify(losA, crossedEdgesA);
			if (losAVerify != NoLineOfSightReason.NONE)
			{
				_Validated = losAVerify;
				_LineOfSight = losA;
				_CrossedEdges = crossedEdgesA;
			}
			else
			{
				Tile[] losB = new Path<Tile>(
					To,
					From,
					i => true,
					(i, j) => 1,
					(i, j) => s.DistanceSquared(i.Center),
					i => i.Neighbors(),
					(i, j) => i == j).Nodes.Reverse().ToArray();
				TileComponentRules[] crossedEdgesB = new TileComponentRules[losB.Length - 1];

				for (int i = 0; i < losB.Length - 1; ++i)
				{
					crossedEdgesB[i] = losB[i].GetEdgeRules(losB[i + 1]);
				}

				NoLineOfSightReason losBVerify = Verify(losB, crossedEdgesB);
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

		static NoLineOfSightReason Verify(Tile[] LineOfSight, TileComponentRules[] CrossedEdges)
		{
			// Always LOS if adjacent.
			if (CrossedEdges.Length <= 1) return NoLineOfSightReason.NONE;

			// Sort LOS so lower tile is first.
			Tile[] los = null;
			TileComponentRules[] edges = null;
			if (LineOfSight[0].RulesCalculator.TrueElevation
				> LineOfSight[LineOfSight.Length - 1].RulesCalculator.TrueElevation)
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
						if (Edges[i] != null && (Edges[i].BlocksLineOfSight || Edges[i].Elevated))
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
						if (Edges[i] != null && (Edges[i].BlocksLineOfSight || Edges[i].Elevated))
							return true;
					}
				}
			}
			return false;
		}

		static bool DepressionBlocks(Tile[] LineOfSight)
		{
			if (LineOfSight[0].RulesCalculator.Depressed)
				return LineOfSight[LineOfSight.Length - 1]
					.RulesCalculator.TrueElevation <= LineOfSight[0].RulesCalculator.TrueElevation
					|| LineOfSight[LineOfSight.Length - 1].RulesCalculator.Depressed;
			return LineOfSight[LineOfSight.Length - 1].RulesCalculator.Depressed;
		}

		static bool ElevationBlocks(Tile[] LineOfSight)
		{
			if (LineOfSight[0].RulesCalculator.TrueElevation
				== LineOfSight[LineOfSight.Length - 1].RulesCalculator.TrueElevation)
			{
				return LineOfSight.Any(
					i => i.RulesCalculator.TrueElevation > LineOfSight[0].RulesCalculator.TrueElevation);
			}
			else
			{
				if (LineOfSight.Length == 3
					&& LineOfSight[0].Configuration.Elevation == LineOfSight[1].Configuration.Elevation) return false;
				for (int i = 0; i < Math.Min(LineOfSight.Length - 1, (LineOfSight.Length - 1) / 2 + 1); ++i)
				{
					if (LineOfSight[i].RulesCalculator.TrueElevation > LineOfSight[0].RulesCalculator.TrueElevation)
						return true;
				}
				return LineOfSight.Any(
					i => i.RulesCalculator.TrueElevation > LineOfSight[0].RulesCalculator.TrueElevation
					&& i.RulesCalculator.TrueElevation > LineOfSight[LineOfSight.Length - 1].RulesCalculator.TrueElevation);
			}
		}
	}
}
