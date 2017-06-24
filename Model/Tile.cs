using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Planar;

using SFML.Window;

namespace PanzerBlitz
{
	public class Tile : Pathable<Tile>
	{
		public EventHandler<EventArgs> OnReconfigure;

		bool _FiredAt;
		bool _CanIndirectFireAt;
		List<Unit> _Units = new List<Unit>();

		int _Elevation;
		TileBase _TileBase;
		List<TilePathOverlay> _PathOverlays;

		public readonly int X;
		public readonly int Y;
		public readonly CollisionPolygon Bounds;

		public readonly Tile[] NeighborTiles = new Tile[6];
		private Edge[] _Edges = new Edge[6];

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
		public Vector2f Center
		{
			get
			{
				return new Vector2f(RealX, RealY);
			}
		}
		public float RealY
		{
			get
			{
				return Y * .75f;
			}
		}
		public float RealX
		{
			get
			{
				return X - (Y % 2 == 0 ? 0 : .5f);
			}
		}
		public int Elevation
		{
			get
			{
				return _Elevation;
			}
			set
			{
				_Elevation = value;
				if (OnReconfigure != null) OnReconfigure(this, EventArgs.Empty);
			}
		}
		public TileBase TileBase
		{
			get
			{
				return _TileBase;
			}
			set
			{
				_TileBase = value;
				if (OnReconfigure != null) OnReconfigure(this, EventArgs.Empty);
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
				new Vector2f(RealX - .5f, RealY + .25f),
				new Vector2f(RealX - .5f, RealY - .25f),
				new Vector2f(RealX, RealY - .5f),
				new Vector2f(RealX + .5f, RealY - .25f),
				new Vector2f(RealX + .5f, RealY + .25f),
				new Vector2f(RealX, RealY + .5f)
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
			return Math.Abs(Node.RealX - RealX) + Math.Abs(Node.RealY - RealY);
		}

		public IEnumerable<Tile> Neighbors()
		{
			foreach (Tile T in NeighborTiles) yield return T;
		}
		// Pathable

		public void SetNeighbor(int Index, Tile Neighbor)
		{
			NeighborTiles[Index] = Neighbor;
		}

		public Edge GetEdge(int Index)
		{
			return _Edges[Index];
		}

		public Edge GetEdge(Tile Neighbor)
		{
			return _Edges[Array.IndexOf(NeighborTiles, Neighbor)];
		}

		public void SetEdge(int Index, Edge Edge)
		{
			_Edges[Index] = Edge;
			if (NeighborTiles[Index] != null && NeighborTiles[Index]._Edges[(Index + 3) % 6] != Edge)
				NeighborTiles[Index].SetEdge((Index + 3) % 6, Edge);
			if (OnReconfigure != null) OnReconfigure(this, EventArgs.Empty);
		}

		public bool HasEdge(Edge Edge)
		{
			return _Edges.Any(i => i == Edge);
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
