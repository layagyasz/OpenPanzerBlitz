using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;
using Cardamom.Planar;
using Cardamom.Serialization;

using SFML.Window;

namespace PanzerBlitz
{
	public class Tile : Pathable<Tile>, Serializable, GameObject
	{
		List<Unit> _Units = new List<Unit>();

		public int Id { get; }
		public readonly Map Map;
		public readonly Coordinate Coordinate;
		public readonly HexCoordinate HexCoordinate;
		public readonly CollisionPolygon Bounds;

		public readonly Tile[] NeighborTiles = new Tile[6];

		public readonly TileConfiguration Configuration;
		public readonly TileRulesCalculator RulesCalculator;
		public readonly TileRuleSet RuleSet;

		Army _ControllingArmy;

		public Vector2f Center
		{
			get
			{
				return new Vector2f(Coordinate.X - (Coordinate.Y % 2 == 0 ? 0 : .5f), Coordinate.Y * .75f);
			}
		}
		public IEnumerable<Unit> Units
		{
			get
			{
				return _Units;
			}
		}
		public Army ControllingArmy
		{
			get
			{
				return _ControllingArmy;
			}
		}

		public Tile(Map Map, Coordinate Coordinate, TileRuleSet RuleSet, IdGenerator IdGenerator)
		{
			this.Map = Map;
			this.Coordinate = Coordinate;
			this.RuleSet = RuleSet;
			Id = IdGenerator.GenerateId();
			HexCoordinate = new HexCoordinate(Coordinate);
			Bounds = CalculateBounds();

			Configuration = new TileConfiguration();
			RulesCalculator = new TileRulesCalculator(this);
			Configuration.OnReconfigure += (sender, e) => RulesCalculator.Recalculate();
		}

		public Tile(SerializationInputStream Stream, Map Map, TileRuleSet RuleSet, IdGenerator IdGenerator)
		{
			this.Map = Map;
			Coordinate = new Coordinate(Stream);
			HexCoordinate = new HexCoordinate(Coordinate);
			Id = IdGenerator.GenerateId();
			Configuration = new TileConfiguration(Stream);
			Bounds = CalculateBounds();

			this.RuleSet = RuleSet;
			RulesCalculator = new TileRulesCalculator(this);
			Configuration.OnReconfigure += (sender, e) => RulesCalculator.Recalculate();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Coordinate);
			Stream.Write(Configuration);
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

		public void FixPaths()
		{
			for (int i = 0; i < 6; ++i)
			{
				if (NeighborTiles[i] == null) continue;
				// There is a disconnected path.
				if (Configuration.GetPathOverlay(i) != TilePathOverlay.NONE
					&& NeighborTiles[i].Configuration.GetPathOverlay((i + 3) % 6) == TilePathOverlay.NONE)
				{
					// Find other tiles disconnected path.
					TilePathOverlay p = Configuration.GetPathOverlay(i);
					Tuple<int, int> otherPath = GetDisconntedNeighbor(p);
					// Connect them.
					if (otherPath != null)
					{
						SetPathOverlay(i, TilePathOverlay.NONE);
						SetPathOverlay(otherPath.Item1, p);

						NeighborTiles[i].SetPathOverlay(otherPath.Item2, TilePathOverlay.NONE);
						NeighborTiles[i].SetPathOverlay((otherPath.Item1 + 3) % 6, p);
					}
					// Could not find another path.
					else
					{
						// Continue in tile on edge.
						if (NeighborTiles[i].NeighborTiles.Any(j => j == null))
						{
							NeighborTiles[i].SetPathOverlay((i + 3) % 6, p);
							NeighborTiles[i].ContinuePath((i + 3) % 6);
						}
						// Otherwise shorten.
						else Configuration.SetPathOverlay(i, TilePathOverlay.NONE);
					}
				}
			}
		}

		Tuple<int, int> GetDisconntedNeighbor(TilePathOverlay Overlay)
		{
			for (int i = 0; i < 6; ++i)
			{
				for (int j = 0; j < 6; ++j)
				{
					if (NeighborTiles[i].Configuration.GetPathOverlay(j) == Overlay
						&& NeighborTiles[i].NeighborTiles[j].Configuration.GetPathOverlay(
							(j + 3) % 6) == TilePathOverlay.NONE)
						return new Tuple<int, int>(i, j);
				}
			}
			return null;
		}

		void ContinuePath(int PathIndex)
		{
			int[] checkSides = { 0, 3, 2, 4, 1, 5 };
			for (int i = 0; i < 6; ++i)
			{
				if (checkSides[i] == PathIndex) continue;
				if (NeighborTiles[checkSides[i]] == null)
				{
					SetPathOverlay(checkSides[i], Configuration.GetPathOverlay(PathIndex));
					return;
				}
			}
		}

		public bool OnEdge(Direction Edge)
		{
			if (Edge == Direction.NONE) return !NeighborTiles.Any(i => i == null);
			if (Edge == Direction.ANY) return NeighborTiles.Any(i => i == null);
			if (Edge == Direction.NORTH)
				return NeighborTiles[(int)Direction.NORTH_WEST] == null
						   && NeighborTiles[(int)Direction.NORTH_EAST] == null;
			if (Edge == Direction.SOUTH)
				return NeighborTiles[(int)Direction.SOUTH_WEST] == null
						   && NeighborTiles[(int)Direction.SOUTH_EAST] == null;
			return NeighborTiles[(int)Edge] == null;
		}

		public OrderInvalidReason CanBeAttacked(AttackMethod AttackMethod)
		{
			if (AttackMethod == AttackMethod.OVERRUN)
			{
				if (Units.Any(i => i.Configuration.UnitClass == UnitClass.FORT))
					return OrderInvalidReason.OVERRUN_FORT;
				if (Configuration.TileBase != TileBase.CLEAR
					|| Configuration.Edges.Any(i => i != TileEdge.NONE)
					|| Configuration.PathOverlays.Any(
						i => i != TilePathOverlay.NONE && !RuleSet.GetRules(i).RoadMove))
					return OrderInvalidReason.OVERRUN_TERRAIN;
			}
			return OrderInvalidReason.NONE;
		}

		public int GetStackSize()
		{
			return _Units.Sum(i => i.GetStackSize());
		}

		public BlockType GetUnitBlockType()
		{
			foreach (Unit u in _Units)
			{
				BlockType t = u.Configuration.GetBlockType();
				if (t == BlockType.NONE || t == BlockType.HARD_BLOCK || t == BlockType.SOFT_BLOCK) return t;
			}
			if (_Units.Count > 0) return BlockType.STANDARD;
			return BlockType.NONE;
		}

		public Tile GetOppositeNeighbor(Tile Neighbor)
		{
			return NeighborTiles[(Array.IndexOf(NeighborTiles, Neighbor) + 3) % 6];
		}

		public void SetNeighbor(int Index, Tile Neighbor)
		{
			NeighborTiles[Index] = Neighbor;
			if (Configuration.GetEdge(Index) != TileEdge.NONE) SetEdge(Index, Configuration.GetEdge(Index));
			Configuration.TriggerReconfigure();
		}

		public void SetPathOverlay(int Index, TilePathOverlay PathOverlay)
		{
			Configuration.SetPathOverlay(Index, PathOverlay);
			if (NeighborTiles[Index] != null
				&& NeighborTiles[Index].Configuration.GetPathOverlay((Index + 3) % 6) != PathOverlay)
				NeighborTiles[Index].SetPathOverlay((Index + 3) % 6, PathOverlay);
		}

		public TilePathOverlay GetPathOverlay(Tile Neighbor)
		{
			return Configuration.GetPathOverlay(Array.IndexOf(NeighborTiles, Neighbor));
		}

		public TileEdge GetEdge(Tile Neighbor)
		{
			return Configuration.GetEdge(Array.IndexOf(NeighborTiles, Neighbor));
		}

		public TileComponentRules GetBaseRules()
		{
			return RuleSet.GetRules(Configuration.TileBase);
		}

		public IEnumerable<TileComponentRules> GetEdgeRules()
		{
			return Configuration.Edges.Select(i => RuleSet.GetRules(i));
		}

		public TileComponentRules GetEdgeRules(int Index)
		{
			return RuleSet.GetRules(Configuration.GetEdge(Index));
		}

		public TileComponentRules GetEdgeRules(Tile Neighbor)
		{
			return RuleSet.GetRules(GetEdge(Neighbor));
		}

		public IEnumerable<TileComponentRules> GetPathOverlayRules()
		{
			return Configuration.PathOverlays.Select(i => RuleSet.GetRules(i));
		}

		public TileComponentRules GetPathOverlayRules(int Index)
		{
			return RuleSet.GetRules(Configuration.GetPathOverlay(Index));
		}

		public TileComponentRules GetPathOverlayRules(Tile Neighbor)
		{
			return RuleSet.GetRules(GetPathOverlay(Neighbor));
		}

		public void SetEdge(int Index, TileEdge Edge)
		{
			Configuration.SetEdge(Index, Edge);
			if (NeighborTiles[Index] != null && NeighborTiles[Index].Configuration.GetEdge((Index + 3) % 6) != Edge)
				NeighborTiles[Index].SetEdge((Index + 3) % 6, Edge);
		}

		public void Control(Unit Unit)
		{
			_ControllingArmy = Unit.Army;
		}

		public void ClearControl(Unit Unit)
		{
			if (_ControllingArmy == Unit.Army && Units.All(i => i.Army != Unit.Army)) _ControllingArmy = null;
		}

		public void Enter(Unit Unit)
		{
			_Units.Add(Unit);
		}

		public void Exit(Unit Unit)
		{
			_Units.Remove(Unit);
		}

		public override string ToString()
		{
			return string.Format("[Tile: Coordinate={0}]", Coordinate);
		}
	}
}
