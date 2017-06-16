using System;
using System.Collections.Generic;

using Cardamom.Graphing;
using Cardamom.Planar;

using SFML.Window;

namespace PanzerBlitz
{
	public class Tile : Pathable<Tile>
	{
		private bool _FiredAt;
		private bool _CanIndirectFireAt;
		private List<Unit> _Units = new List<Unit>();

		public readonly int Elevation;
		private TileConfiguration _TileConfiguration;

		public readonly int X;
		public readonly int Y;
		public readonly CollisionPolygon Bounds;

		public readonly Tile[] NeighborTiles = new Tile[6];
		private Edge[] _Edges = new Edge[6];

		// Overlays
		private bool _River;
		private bool _RiverFord;
		private bool _Road;
		private bool _Forest;

		public bool FiredAt
		{
			get
			{
				return _FiredAt;
			}
		}
		public bool CanIndirectFireAt
		{
			get
			{
				return _CanIndirectFireAt;
			}
		}
		public bool River
		{
			get
			{
				return _River;
			}
		}
		public bool RiverFord
		{
			get
			{
				return _RiverFord;
			}
		}
		public bool Road
		{
			get
			{
				return _Road;
			}
		}
		public bool Forest
		{
			get
			{
				return _Forest;
			}
		}
		public Vector2f Center
		{
			get
			{
				return new Vector2f(OffsetX, OffsetY);
			}
		}
		public float OffsetY
		{
			get
			{
				return Y * .75f;
			}
		}
		public float OffsetX
		{
			get
			{
				return X + (Y % 2 == 0 ? 0 : .5f);
			}
		}
		public TileConfiguration TileConfiguration
		{
			get
			{
				return _TileConfiguration;
			}
		}
		public IEnumerable<Unit> Units
		{
			get
			{
				return _Units;
			}
		}

		public Tile(int X, int Y)
		{
			this.X = X;
			this.Y = Y;
			Bounds = new CollisionPolygon(
				new Vector2f[]
			{
				new Vector2f(OffsetX - .5f, OffsetY + .25f),
				new Vector2f(OffsetX - .5f, OffsetY - .25f),
				new Vector2f(OffsetX, OffsetY - .5f),
				new Vector2f(OffsetX + .5f, OffsetY - .25f),
				new Vector2f(OffsetX + .5f, OffsetY + .25f),
				new Vector2f(OffsetX, OffsetY + .5f)
			});
		}

		// Pathable
		public bool Passable { get { return true; } }

		public double DistanceTo(Tile Node)
		{
			return 1;
		}

		public double HeuristicDistanceTo(Tile Node)
		{
			return Math.Abs(Node.OffsetX - OffsetX) + Math.Abs(Node.OffsetY - OffsetY);
		}

		public IEnumerable<Tile> Neighbors()
		{
			foreach (Tile T in NeighborTiles) yield return T;
		}
		// Pathable

		public LineOfSight CalculateLineOfSight(Tile Tile)
		{
			Segment los = new Segment(Center, Tile.Center);
			Tile current = this;
			List<Tile> path = new List<Tile>();
			path.Add(current);
			while (current != Tile)
			{
				Tile n = Array.Find(current.NeighborTiles, i => i.Bounds.Intersects(los));
				current = n;
				path.Add(n);
			}
			return new LineOfSight(path);
		}

		public void Reconfigure(TileConfiguration TileConfiguration)
		{
			_TileConfiguration = TileConfiguration;
		}

		public void SetNeighbor(int Index, Tile Neighbor)
		{
			NeighborTiles[Index] = Neighbor;
		}

		public Edge GetEdge(Tile Neighbor)
		{
			return _Edges[Array.IndexOf(NeighborTiles, Neighbor)];
		}

		public void Enter(Unit Unit)
		{
			_Units.Add(Unit);
		}

		public void Exit(Unit Unit)
		{
			_Units.Remove(Unit);
		}

		public void FireAt()
		{
			_FiredAt = true;
		}

		public void MakeCanIndirectFireAt()
		{
			_CanIndirectFireAt = true;
		}

		public void Reset()
		{
			_FiredAt = false;
			_CanIndirectFireAt = false;
		}
	}
}
