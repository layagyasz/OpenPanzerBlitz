using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;

namespace PanzerBlitz
{
	public class LineOfSight
	{
		Tile[] _LineOfSight;
		Edge[] _CrossedEdges;

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
			Tile[] losA = new Path<Tile>(
				From,
				To,
				i => true,
				(i, j) => 1,
				(i, j) => i.HeuristicDistanceTo(j),
				i => i.Neighbors(),
				(i, j) => i == j).Nodes.ToArray();
			Edge[] crossedEdgesA = new Edge[losA.Length - 1];

			for (int i = 0; i < losA.Length - 1; ++i)
			{
				crossedEdgesA[i] = losA[i].GetEdge(losA[i + 1]);
			}

			Tile[] losB = new Path<Tile>(
				To,
				From,
				i => true,
				(i, j) => 1,
				(i, j) => i.HeuristicDistanceTo(j),
				i => i.Neighbors(),
				(i, j) => i == j).Nodes.Reverse().ToArray();
			Edge[] crossedEdgesB = new Edge[losB.Length - 1];

			for (int i = 0; i < losB.Length - 1; ++i)
			{
				crossedEdgesB[i] = losB[i].GetEdge(losB[i + 1]);
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

		static NoLineOfSightReason Verify(Tile[] LineOfSight, Edge[] CrossedEdges)
		{
			// Always LOS if adjacent.
			if (CrossedEdges.Length == 1) return NoLineOfSightReason.NONE;

			// Sort LOS so lower tile is first.
			Tile[] los = null;
			Edge[] edges = null;
			if (LineOfSight[0].Configuration.TrueElevation
				> LineOfSight[LineOfSight.Length - 1].Configuration.TrueElevation)
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
			if (EdgeBlocks(los, edges)) return NoLineOfSightReason.TERRAIN;
			return NoLineOfSightReason.NONE;
		}

		static bool EdgeBlocks(Tile[] LineOfSight, Edge[] Edges)
		{
			if (LineOfSight[0].Elevation == LineOfSight[LineOfSight.Length - 1].Elevation)
			{
				for (int i = 0; i < LineOfSight.Length - 1; ++i)
				{
					if (Edges[i] != null
						&& (Edges[i].BlocksLineOfSight || Edges[i].Elevated)
						&& (LineOfSight[0].Elevation <= LineOfSight[i + 1].Elevation))
						return true;
				}
			}
			else
			{
				if (Edges[0] != null && Edges[0].BlocksLineOfSight) return true;
				for (int i = 0; i < LineOfSight.Length - 1; ++i)
				{
					if (LineOfSight[i + 1].Elevation > LineOfSight[0].Elevation
						&& Edges[i] != null && (Edges[i].BlocksLineOfSight || Edges[i].Elevated))
						return true;
				}
			}
			return false;
		}

		static bool DepressionBlocks(Tile[] LineOfSight)
		{
			if (LineOfSight[0].Configuration.Depressed)
				return LineOfSight[LineOfSight.Length - 1]
					.Configuration.TrueElevation <= LineOfSight[0].Configuration.TrueElevation
					|| LineOfSight[LineOfSight.Length - 1].Configuration.Depressed;
			return LineOfSight[LineOfSight.Length - 1].Configuration.Depressed;
		}

		static bool ElevationBlocks(Tile[] LineOfSight)
		{
			if (LineOfSight[0].Configuration.TrueElevation
				== LineOfSight[LineOfSight.Length - 1].Configuration.TrueElevation)
			{
				return LineOfSight.Any(
					i => i.Configuration.TrueElevation > LineOfSight[0].Configuration.TrueElevation);
			}
			else
			{
				if (LineOfSight.Length == 3 && LineOfSight[0].Elevation == LineOfSight[1].Elevation) return false;
				for (int i = 0; i < Math.Min(LineOfSight.Length - 1, (LineOfSight.Length - 1) / 2 + 1); ++i)
				{
					if (LineOfSight[i].Configuration.TrueElevation > LineOfSight[0].Configuration.TrueElevation)
						return true;
				}
				return LineOfSight.Any(
					i => i.Configuration.TrueElevation > LineOfSight[0].Configuration.TrueElevation
					&& i.Configuration.TrueElevation > LineOfSight[LineOfSight.Length - 1].Configuration.TrueElevation);
			}
		}
	}
}
