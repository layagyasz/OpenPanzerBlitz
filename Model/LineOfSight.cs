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

		NoLineOfSightReason _Verified;

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

			for (int i = 0; i < losA.Length - 1; ++i)
			{
				crossedEdgesB[i] = losB[i].GetEdge(losB[i + 1]);
			}

			NoLineOfSightReason losAVerify = Verify(losA, crossedEdgesA);
			if (losAVerify != NoLineOfSightReason.NONE)
			{
				_Verified = losAVerify;
				_LineOfSight = losA;
				_CrossedEdges = crossedEdgesA;
			}
			else
			{
				NoLineOfSightReason losBVerify = Verify(losB, crossedEdgesB);
				if (losBVerify == NoLineOfSightReason.NONE)
				{
					_Verified = losBVerify;
					_LineOfSight = losB;
					_CrossedEdges = crossedEdgesB;
				}
				else
				{
					_Verified = losAVerify;
					_LineOfSight = losA;
					_CrossedEdges = crossedEdgesA;
				}
			}
		}

		public NoLineOfSightReason Verify()
		{
			return _Verified;
		}

		static NoLineOfSightReason Verify(Tile[] LineOfSight, Edge[] CrossedEdges)
		{
			// Always LOS if adjacent.
			if (CrossedEdges.Length == 1) return NoLineOfSightReason.NONE;

			// Sort LOS so lower tile is first.
			Tile[] los = null;
			Edge[] edges = null;
			if (LineOfSight[0].Elevation > LineOfSight[LineOfSight.Length - 1].Elevation)
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
			if (ElevationBlocks(los)) return NoLineOfSightReason.SLOPE;
			// if (GullyBlocks(los, edges)) return NoLineOfSightReason.GULLY;
			if (EdgeBlocks(los, edges, Edge.FOREST)) return NoLineOfSightReason.FOREST;
			if (EdgeBlocks(los, edges, Edge.TOWN)) return NoLineOfSightReason.TOWN;
			if (EdgeBlocks(los, edges, Edge.SLOPE)) return NoLineOfSightReason.SLOPE;
			return NoLineOfSightReason.NONE;
		}

		static bool EdgeBlocks(Tile[] LineOfSight, Edge[] Edges, Edge EdgeType)
		{
			if (LineOfSight[0].Elevation == LineOfSight[LineOfSight.Length - 1].Elevation)
			{
				for (int i = 0; i < LineOfSight.Length - 1; ++i)
				{
					if (Edges[i] == EdgeType && (LineOfSight[0].Elevation == LineOfSight[i].Elevation
												 || LineOfSight[0].Elevation == LineOfSight[i + 1].Elevation))
						return true;
				}
			}
			else
			{
				if (Edges[0] == EdgeType) return true;
				for (int i = 0; i < LineOfSight.Length - 1; ++i)
				{
					if (LineOfSight[i + 1].Elevation > LineOfSight[0].Elevation && Edges[i] == EdgeType)
						return true;
				}
			}
			return false;
		}

		static bool ElevationBlocks(Tile[] LineOfSight)
		{
			if (LineOfSight[0].Elevation == LineOfSight[LineOfSight.Length - 1].Elevation)
			{
				return LineOfSight.Any(i => i.Elevation > LineOfSight[0].Elevation);
			}
			else
			{
				for (int i = 1; i < Math.Min(LineOfSight.Length - 1, (LineOfSight.Length - 1) / 2 + 2); ++i)
				{
					if (LineOfSight[i].Elevation > LineOfSight[0].Elevation) return true;
				}
			}
			return false;
		}
	}
}
