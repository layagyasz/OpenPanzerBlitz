﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class LineOfSight
	{
		Tile[] _LineOfSight;
		Edge[] _CrossedEdges;

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

		public LineOfSight(IEnumerable<Tile> LineOfSight)
		{
			_LineOfSight = LineOfSight.ToArray();
			_CrossedEdges = new Edge[_LineOfSight.Length - 1];
			for (int i = 0; i < _LineOfSight.Length - 1; ++i)
			{
				_CrossedEdges[i] = _LineOfSight[i].GetEdge(_LineOfSight[i + 1]);
			}
		}

		public NoLineOfSightReason Verify()
		{
			// Always LOS if adjacent.
			if (_CrossedEdges.Length == 1) return NoLineOfSightReason.NONE;

			// Sort LOS so lower tile is first.
			Tile[] los = null;
			Edge[] edges = null;
			if (Initial.Elevation > Final.Elevation)
			{
				los = _LineOfSight.Reverse().ToArray();
				edges = _CrossedEdges.Reverse().ToArray();
			}
			else
			{
				los = _LineOfSight.ToArray();
				edges = _CrossedEdges.ToArray();
			}

			// Check for blocks.
			if (ElevationBlocks(los)) return NoLineOfSightReason.SLOPE;
			// if (GullyBlocks(los, edges)) return NoLineOfSightReason.GULLY;
			if (EdgeBlocks(los, edges, Edge.FOREST)) return NoLineOfSightReason.FOREST;
			if (EdgeBlocks(los, edges, Edge.TOWN)) return NoLineOfSightReason.TOWN;
			if (EdgeBlocks(los, edges, Edge.SLOPE)) return NoLineOfSightReason.SLOPE;
			return NoLineOfSightReason.NONE;
		}

		public static bool EdgeBlocks(Tile[] LineOfSight, Edge[] Edges, Edge EdgeType)
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

		public static bool ElevationBlocks(Tile[] LineOfSight)
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
