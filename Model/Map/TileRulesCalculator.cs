using System;
using System.Linq;

namespace PanzerBlitz
{
	public class TileRulesCalculator
	{
		public readonly Tile Tile;

		bool _MustAttackAllUnits;
		bool _TreatUnitsAsArmored;
		bool _Depressed;
		bool _DepressedTransition;
		bool _Concealing;
		int _DieModifier;
		int _TrueElevation;

		public bool MustAttackAllUnits
		{
			get
			{
				return _MustAttackAllUnits;
			}
		}
		public bool TreatUnitsAsArmored
		{
			get
			{
				return _TreatUnitsAsArmored;
			}
		}
		public bool Depressed
		{
			get
			{
				return _Depressed;
			}
		}
		public bool DepressedTransition
		{
			get
			{
				return _DepressedTransition;
			}
		}
		public bool Concealing
		{
			get
			{
				return _Concealing;
			}
		}
		public int DieModifier
		{
			get
			{
				return _DieModifier;
			}
		}
		public int TrueElevation
		{
			get
			{
				return _TrueElevation;
			}
		}

		public TileRulesCalculator(Tile Tile)
		{
			this.Tile = Tile;
			Recalculate();
		}

		public bool CanMove(Unit Unit, Tile To, bool RoadMovement, bool IgnoreOccupyingUnits = false)
		{
			return Math.Abs(float.MaxValue - GetMoveCost(Unit, To, RoadMovement, IgnoreOccupyingUnits)) > float.Epsilon;
		}

		public float GetMoveCost(Unit Unit, Tile To, bool RoadMovement, bool IgnoreOccupyingUnits = false)
		{
			if (!IgnoreOccupyingUnits && Unit.CanEnter(To) == OrderInvalidReason.TILE_ENEMY_OCCUPIED)
				return float.MaxValue;

			BlockType toBlock = To.GetUnitBlockType();
			BlockType fromBlock = Tile.GetUnitBlockType();

			bool adjacent = !Unit.Moved && To.NeighborTiles.Any(i => i != null && i.Units.Contains(Unit));

			if (toBlock == BlockType.HARD_BLOCK && !adjacent)
				return float.MaxValue;
			if ((fromBlock == BlockType.HARD_BLOCK || fromBlock == BlockType.SOFT_BLOCK) && Unit.Moved)
				return float.MaxValue;

			bool useRoadMovement = RoadMovement
				&& !Unit.Configuration.MovementRules.CannotUseRoadMovement
				&& (toBlock == BlockType.CLEAR || (Tile == Unit.Position && Unit.IsSolitary()));

			return CalculateMovement(
				Tile,
				To,
				Tile.GetEdgeRules(To),
				Tile.GetPathOverlayRules(To),
				useRoadMovement,
				adjacent,
				Unit.Moved,
				Unit.Configuration.MovementRules);
		}

		public void Recalculate()
		{
			if (Tile.RuleSet == null) return;

			_MustAttackAllUnits = Tile.GetBaseRules().MustAttackAllUnits
									  || Tile.GetEdgeRules().Any(i => i != null && i.MustAttackAllUnits)
									  || Tile.GetPathOverlayRules().Any(i => i != null && i.MustAttackAllUnits);
			_TreatUnitsAsArmored = Tile.GetBaseRules().TreatUnitsAsArmored
									   || Tile.GetEdgeRules().Any(i => i != null && i.TreatUnitsAsArmored)
									   || Tile.GetPathOverlayRules().Any(i => i != null && i.TreatUnitsAsArmored);
			_Depressed = Tile.GetBaseRules().Depressed
							 || Tile.GetEdgeRules().Any(i => i != null && i.Depressed)
							 || Tile.GetPathOverlayRules().Any(i => i != null && i.Depressed);
			_DepressedTransition = Tile.GetBaseRules().DepressedTransition
									   || Tile.GetEdgeRules().Any(i => i != null && i.DepressedTransition)
									   || Tile.GetPathOverlayRules().Any(i => i != null && i.DepressedTransition);
			_Concealing = Tile.GetBaseRules().Concealing
							  || Tile.GetEdgeRules().Any(i => i != null && i.Concealing)
							  || Tile.GetPathOverlayRules().Any(i => i != null && i.Concealing);

			_DieModifier =
				Math.Max(
					Tile.GetBaseRules().DieModifier,
					Math.Max(
						Tile.GetEdgeRules().Max(i => i == null ? 0 : i.DieModifier),
						Tile.GetPathOverlayRules().Max(i => i == null ? 0 : i.DieModifier)));
			_TrueElevation = 2 * Tile.Configuration.Elevation
									 + (Tile.GetBaseRules().Elevated
										|| Tile.GetEdgeRules().Any(i => i != null && i.Elevated)
										|| Tile.GetPathOverlayRules().Any(i => i != null && i.Elevated) ? 1 : 0);
		}

		float CalculateMovement(
			Tile From,
			Tile To,
			TileComponentRules Edge,
			TileComponentRules Path,
			bool CanUseRoadMovement,
			bool Adjacent,
			bool UnitMoved,
			UnitMovementRules MovementRules)
		{
			bool roaded = Path != null && Path.RoadMove;

			if (CanUseRoadMovement && roaded && Path.BaseMoveCost > 0) return Path.BaseMoveCost;

			bool leavingDepressed = From.RulesCalculator.Depressed
										&& (Path == null || !Path.Depressed)
										&& (Path == null || !Path.DepressedTransition)
										&& !roaded;

			float leaveCost = 0;
			if (leavingDepressed)
			{
				float maxLeaveCost = 0;
				for (int i = 0; i < 6; ++i)
				{
					TileComponentRules rules = From.GetPathOverlayRules(i);
					if (rules != null) maxLeaveCost = Math.Max(maxLeaveCost, rules.BaseLeaveCost);
				}
				leaveCost = GetBlockTypeMoveCost(MovementRules.Sloped, Adjacent, UnitMoved)
					+ GetBlockTypeMoveCost(MovementRules.Rough, Adjacent, UnitMoved)
					+ maxLeaveCost;
			}

			float crossCost = GetRulesMoveCost(Edge, MovementRules, Adjacent, UnitMoved, roaded, true);

			float enterCost = GetRulesMoveCost(To.GetBaseRules(), MovementRules, Adjacent, UnitMoved, roaded, false);

			float edgeCost = float.MaxValue;
			for (int i = 0; i < 6; ++i)
			{
				TileComponentRules e = To.GetEdgeRules(i);
				if (e == null) continue;

				float eMove = GetRulesMoveCost(e, MovementRules, Adjacent, UnitMoved, roaded, false);
				if (eMove > 0) edgeCost = Math.Min(edgeCost, eMove);
			}
			float pathCost = 0;
			if (Path != null && (!Path.RoadMove || CanUseRoadMovement))
				pathCost = GetRulesMoveCost(Path, MovementRules, Adjacent, UnitMoved, roaded, false);

			enterCost += crossCost;
			if (edgeCost < float.MaxValue) enterCost = edgeCost + crossCost;
			if (To.RulesCalculator.Depressed)
				enterCost += GetBlockTypeMoveCost(MovementRules.Depressed, Adjacent, UnitMoved);

			if (pathCost > 0) enterCost = pathCost;

			return enterCost + leaveCost;
		}

		float GetRulesMoveCost(
			TileComponentRules TileRules,
			UnitMovementRules MovementRules,
			bool Adjacent,
			bool UnitMoved,
			bool Roaded,
			bool IsEdge)
		{
			if (TileRules == null) return 0;

			float cost = 0;
			if (TileRules.Water && !Roaded) cost += GetBlockTypeMoveCost(MovementRules.Water, Adjacent, UnitMoved);
			if (IsEdge)
			{
				if (TileRules.DenseEdge && !Roaded)
					cost += GetBlockTypeMoveCost(MovementRules.DenseEdge, Adjacent, UnitMoved);
				return cost;
			}
			cost += TileRules.BaseMoveCost;
			if (TileRules.Depressed && !Roaded)
				cost += GetBlockTypeMoveCost(MovementRules.Depressed, Adjacent, UnitMoved);
			if (TileRules.Elevated) cost += GetBlockTypeMoveCost(MovementRules.Sloped, Adjacent, UnitMoved);
			if (TileRules.Rough) cost += GetBlockTypeMoveCost(MovementRules.Rough, Adjacent, UnitMoved);
			if (TileRules.Swamp) cost += GetBlockTypeMoveCost(MovementRules.Swamp, Adjacent, UnitMoved);
			return cost;
		}

		float GetBlockTypeMoveCost(BlockType BlockType, bool Adjacent, bool UnitMoved)
		{
			switch (BlockType)
			{
				case BlockType.CLEAR: return 0f;
				case BlockType.HARD_BLOCK: return Adjacent ? (UnitMoved ? float.MaxValue : 0f) : float.MaxValue;
				case BlockType.IMPASSABLE: return float.MaxValue;
				case BlockType.SLOW: return 1;
				case BlockType.SOFT_BLOCK: return UnitMoved ? float.MaxValue : 0;
				case BlockType.SPEED: return -1;
				case BlockType.STANDARD: return 0;
				default: throw new Exception(string.Format("No movement cost for {0}", BlockType));
			}
		}
	}
}
