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

		public readonly Coordinate Coordinate;
		public readonly HexCoordinate HexCoordinate;
		public readonly CollisionPolygon Bounds;

		public readonly Tile[] NeighborTiles = new Tile[6];
		Edge[] _Edges = new Edge[6];
		TilePathOverlay[] _PathOverlays = new TilePathOverlay[6];

		public readonly TileConfiguration Configuration;

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
				return new Vector2f(Coordinate.X - (Coordinate.Y % 2 == 0 ? 0 : .5f), Coordinate.Y * .75f);
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

		public Tile(Coordinate Coordinate)
		{
			this.Coordinate = Coordinate;
			HexCoordinate = new HexCoordinate(Coordinate);
			Bounds = CalculateBounds();

			Configuration = new TileConfiguration(this);
			OnReconfigure += (sender, e) => Configuration.Recalculate();
		}

		public Tile(Coordinate Coordinate, Tile Copy, bool Invert = false)
				: this(Coordinate)
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

			OnReconfigure += (sender, e) => Configuration.Recalculate();
		}

		public Tile(SerializationInputStream Stream)
		{
			Coordinate = new Coordinate(Stream);
			HexCoordinate = new HexCoordinate(Coordinate);
			_Elevation = Stream.ReadByte();
			_TileBase = TileBase.TILE_BASES[Stream.ReadByte()];
			_Edges = Stream.ReadArray(i => Edge.EDGES[Stream.ReadByte()]);
			_PathOverlays = Stream.ReadArray(i => TilePathOverlay.PATH_OVERLAYS[Stream.ReadByte()]);
			Bounds = CalculateBounds();

			Configuration = new TileConfiguration(this);
			OnReconfigure += (sender, e) => Configuration.Recalculate();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Coordinate);
			Stream.Write((byte)_Elevation);
			Stream.Write((byte)Array.IndexOf(TileBase.TILE_BASES, _TileBase));
			Stream.Write(_Edges, i => Stream.Write((byte)Array.IndexOf(Edge.EDGES, i)));
			Stream.Write(_PathOverlays, i => Stream.Write((byte)Array.IndexOf(TilePathOverlay.PATH_OVERLAYS, i)));
		}

		private CollisionPolygon CalculateBounds()
		{
			Vector2f c = Center;
			return new CollisionPolygon(
				new Vector2f[]
			{
				new Vector2f(c.X - .5f, c.Y + .25f),
				new Vector2f(c.X - .5f, c.Y - .25f),
				new Vector2f(c.X, c.Y - .5f),
				new Vector2f(c.X + .5f, c.Y - .25f),
				new Vector2f(c.X + .5f, c.Y + .25f),
				new Vector2f(c.X, c.Y + .5f)
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
			return .49 * HexCoordinate.Distance(Node.HexCoordinate);
		}

		public IEnumerable<Tile> Neighbors()
		{
			foreach (Tile T in NeighborTiles) yield return T;
		}
		// Pathable

		public bool OnEdge(Direction Edge)
		{
			if (Edge == Direction.NONE) return true;
			if (Edge == Direction.ANY) return NeighborTiles.Any(i => i == null);
			if (Edge == Direction.NORTH)
				return NeighborTiles[(int)Direction.NORTH_WEST] == null
						   && NeighborTiles[(int)Direction.NORTH_EAST] == null;
			if (Edge == Direction.SOUTH)
				return NeighborTiles[(int)Direction.SOUTH_WEST] == null
						   && NeighborTiles[(int)Direction.SOUTH_EAST] == null;
			return NeighborTiles[(int)Edge] == null;
		}

		public NoAttackReason CanBeAttacked(AttackMethod AttackMethod)
		{
			if (AttackMethod == AttackMethod.OVERRUN)
			{
				if (Units.Any(i => i.Configuration.UnitClass == UnitClass.FORT)) return NoAttackReason.OVERRUN_FORT;
				if (TileBase != TileBase.CLEAR
					|| Edges.Any(i => i != null)
					|| PathOverlays.Any(i => i != null && !i.RoadMove))
					return NoAttackReason.OVERRUN_TERRAIN;
			}
			return NoAttackReason.NONE;
		}

		public int GetStackSize()
		{
			return _Units.Sum(i => i.GetStackSize());
		}

		public BlockType GetBlockType()
		{
			foreach (Unit u in _Units)
			{
				BlockType t = u.Configuration.GetBlockType();
				if (t == BlockType.CLEAR || t == BlockType.HARD_BLOCK || t == BlockType.SOFT_BLOCK) return t;
			}
			if (_Units.Count > 0) return BlockType.STANDARD;
			return BlockType.CLEAR;
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
