using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Planar;
using Cardamom.Serialization;

using SFML.Window;

namespace PanzerBlitz
{
	public class Tile : Pathable<Tile>, Serializable
	{
		public EventHandler<EventArgs> OnReconfigure;
		List<Unit> _Units = new List<Unit>();

		int _Elevation;
		TileBase _TileBase;

		public readonly int X;
		public readonly int Y;
		public readonly CollisionPolygon Bounds;

		public readonly Tile[] NeighborTiles = new Tile[6];
		Edge[] _Edges = new Edge[6];
		TilePathOverlay[] _PathOverlays = new TilePathOverlay[6];

		public readonly MovementProfile MovementProfile;

		bool _FiredAt;
		bool _CanIndirectFireAt;

		public IEnumerable<Edge> Edges
		{
			get
			{
				return _Edges;
			}
		}
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
		public IEnumerable<TilePathOverlay> PathOverlays
		{
			get
			{
				return _PathOverlays;
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
			Bounds = CalculateBounds();

			MovementProfile = new MovementProfile(this);
			OnReconfigure += (sender, e) => MovementProfile.Recalculate();
		}

		public Tile(int X, int Y, Tile Copy, bool Invert = false)
				: this(X, Y)
		{
			_Elevation = Copy.Elevation;
			_TileBase = Copy.TileBase;

			if (Invert)
			{
				for (int i = 0; i < 6; ++i)
				{
					_Edges[i] = Copy._Edges[(i + 3) % 6];
					_PathOverlays[i] = Copy._PathOverlays[(i + 3) % 6];
				}
			}
			else
			{
				_Edges = Copy._Edges.ToArray();
				_PathOverlays = Copy._PathOverlays.ToArray();
			}

			OnReconfigure += (sender, e) => MovementProfile.Recalculate();
		}

		public Tile(SerializationInputStream Stream)
		{
			X = Stream.ReadInt32();
			Y = Stream.ReadInt32();
			_Elevation = Stream.ReadByte();
			_TileBase = TileBase.TILE_BASES[Stream.ReadByte()];
			_Edges = Stream.ReadArray(i => Edge.EDGES[Stream.ReadByte()]);
			_PathOverlays = Stream.ReadArray(i => TilePathOverlay.PATH_OVERLAYS[Stream.ReadByte()]);
			Bounds = CalculateBounds();

			MovementProfile = new MovementProfile(this);
			OnReconfigure += (sender, e) => MovementProfile.Recalculate();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(X);
			Stream.Write(Y);
			Stream.Write((byte)_Elevation);
			Stream.Write((byte)Array.IndexOf(TileBase.TILE_BASES, _TileBase));
			Stream.Write(_Edges, i => Stream.Write((byte)Array.IndexOf(Edge.EDGES, i)));
			Stream.Write(_PathOverlays, i => Stream.Write((byte)Array.IndexOf(TilePathOverlay.PATH_OVERLAYS, i)));
		}

		private CollisionPolygon CalculateBounds()
		{
			return new CollisionPolygon(
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
			return .5f * (Math.Abs(Node.RealX - RealX) + Math.Abs(Node.RealY - RealY));
		}

		public IEnumerable<Tile> Neighbors()
		{
			foreach (Tile T in NeighborTiles) yield return T;
		}
		// Pathable

		public bool IsEnemyOccupied(Army Army)
		{
			return Units.Any(i => i.Army != Army);
		}

		public Tile GetOppositeNeighbor(Tile Neighbor)
		{
			return NeighborTiles[(Array.IndexOf(NeighborTiles, Neighbor) + 3) % 6];
		}

		public void SetNeighbor(int Index, Tile Neighbor)
		{
			NeighborTiles[Index] = Neighbor;
			if (_Edges[Index] != null) SetEdge(Index, _Edges[Index]);
			if (OnReconfigure != null) OnReconfigure(this, EventArgs.Empty);
		}

		public void SetPathOverlay(int Index, TilePathOverlay PathOverlay)
		{
			_PathOverlays[Index] = PathOverlay;
			if (NeighborTiles[Index] != null && NeighborTiles[Index]._PathOverlays[(Index + 3) % 6] != PathOverlay)
				NeighborTiles[Index].SetPathOverlay((Index + 3) % 6, PathOverlay);
			if (OnReconfigure != null) OnReconfigure(this, EventArgs.Empty);
		}

		public TilePathOverlay GetPathOverlay(int Index)
		{
			return _PathOverlays[Index];
		}

		public TilePathOverlay GetPathOverlay(Tile Neighbor)
		{
			return _PathOverlays[Array.IndexOf(NeighborTiles, Neighbor)];
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
