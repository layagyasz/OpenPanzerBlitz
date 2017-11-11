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

			bool unitMoved = !Unit.Moved && Tile.Units.Contains(Unit);
			bool adjacent = !unitMoved && !Unit.Moved && To.NeighborTiles.Any(i => i != null && i.Units.Contains(Unit));

			if (toBlock == BlockType.HARD_BLOCK && !adjacent)
				return float.MaxValue;
			if ((fromBlock == BlockType.HARD_BLOCK || fromBlock == BlockType.SOFT_BLOCK) && unitMoved)
				return float.MaxValue;

			bool useRoadMovement = RoadMovement
						&& !Tile.Map.Environment.IsRoadMovementRestricted(Unit.Configuration.UnitClass)
						&& !Unit.Configuration.MovementRules.CannotUseRoadMovement
						&& (toBlock == BlockType.NONE || (Tile == Unit.Position && Unit.IsSolitary()));
			if (Unit.Configuration.CannotUseRoadMovementWithOversizedPassenger
				&& Unit.Passenger != null
				&& Unit.Passenger.Configuration.IsOversizedPassenger)
				useRoadMovement = false;

			float multiplier = Unit.Passenger != null && Unit.Passenger.Configuration.IsOversizedPassenger
								   ? Unit.Configuration.OversizedPassengerMovementMultiplier : 1;

			return multiplier * CalculateMovement(
				Tile,
				To,
				Tile.GetEdgeRules(To),
				Tile.GetPathOverlayRules(To),
				useRoadMovement,
				adjacent,
				unitMoved,
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
			_TrueElevation = Tile.Configuration.Elevation
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

			bool leavingDepressed = From.RulesCalculator.Depressed
										&& (Path == null || !Path.Depressed)
										&& (Path == null || !Path.DepressedTransition)
										&& !roaded;

			float leaveCost = 0;
			if (leavingDepressed)
			{
				leaveCost = 1 + GetMoveCost(MovementRules.Sloped, Adjacent, UnitMoved)
					+ GetMoveCost(MovementRules.Rough, Adjacent, UnitMoved)
					+ GetMoveCost(MovementRules.Uphill, Adjacent, UnitMoved);
			}

			float crossCost = GetRulesMoveCost(
				Edge, MovementRules, Adjacent, UnitMoved, roaded, CanUseRoadMovement, true);

			float enterCost = GetRulesMoveCost(
				To.GetBaseRules(), MovementRules, Adjacent, UnitMoved, roaded, CanUseRoadMovement, false);

			float edgeCost = float.MaxValue;
			for (int i = 0; i < 6; ++i)
			{
				float eMove = GetRulesMoveCost(
					To.GetEdgeRules(i), MovementRules, Adjacent, UnitMoved, roaded, CanUseRoadMovement, false);
				if (eMove > 0) edgeCost = Math.Min(edgeCost, eMove);
			}
			float pathCost = 0;
			if (Path != null && (!Path.RoadMove || CanUseRoadMovement))
				pathCost = GetRulesMoveCost(
					Path, MovementRules, Adjacent, UnitMoved, roaded, CanUseRoadMovement, false);

			if (edgeCost < float.MaxValue) enterCost = edgeCost;
			if (pathCost > 0) enterCost = pathCost;
			if (To.RulesCalculator.Depressed && !roaded)
				enterCost += GetMoveCost(MovementRules.Depressed, Adjacent, UnitMoved);
			if (From.RulesCalculator.TrueElevation < To.RulesCalculator.TrueElevation)
				enterCost += GetMoveCost(MovementRules.Uphill, Adjacent, UnitMoved);
			if (From.Configuration.Elevation > To.Configuration.Elevation)
				enterCost += GetMoveCost(MovementRules.Downhill, Adjacent, UnitMoved);

			return enterCost + leaveCost + crossCost;
		}

		float GetRulesMoveCost(
			TileComponentRules TileRules,
			UnitMovementRules MovementRules,
			bool Adjacent,
			bool UnitMoved,
			bool Roaded,
			bool UseRoad,
			bool IsEdge)
		{
			if (TileRules == null) return 0;

			float cost = IsEdge ? 0 : 1;
			if (TileRules.Frozen && !Roaded) cost += GetMoveCost(MovementRules.Frozen, Adjacent, UnitMoved);
			if (TileRules.Water && !Roaded) cost += GetMoveCost(MovementRules.Water, Adjacent, UnitMoved);
			if (IsEdge)
			{
				if (TileRules.DenseEdge && !Roaded)
					cost += GetMoveCost(MovementRules.DenseEdge, Adjacent, UnitMoved);
				return cost;
			}
			if (TileRules.Depressed && !Roaded)
				cost += GetMoveCost(MovementRules.Depressed, Adjacent, UnitMoved);
			if (TileRules.Elevated) cost += GetMoveCost(MovementRules.Sloped, Adjacent, UnitMoved);
			if (TileRules.Loose) cost += GetMoveCost(MovementRules.Loose, Adjacent, UnitMoved);
			if (TileRules.Roaded && UseRoad) cost += GetMoveCost(MovementRules.Roaded, Adjacent, UnitMoved);
			if (TileRules.Rough) cost += GetMoveCost(MovementRules.Rough, Adjacent, UnitMoved);
			if (TileRules.Swamp) cost += GetMoveCost(MovementRules.Swamp, Adjacent, UnitMoved);
			return cost;
		}

		float GetMoveCost(MovementCost MovementCost, bool Adjacent, bool UnitMoved)
		{
			if (MovementCost.BlockType == BlockType.NONE) return MovementCost.Cost;
			return GetBlockTypeMoveCost(MovementCost.BlockType, Adjacent, UnitMoved);
		}

		float GetBlockTypeMoveCost(BlockType BlockType, bool Adjacent, bool UnitMoved)
		{
			switch (BlockType)
			{
				case BlockType.NONE: return 0f;
				case BlockType.HARD_BLOCK: return Adjacent ? (UnitMoved ? float.MaxValue : 0f) : float.MaxValue;
				case BlockType.IMPASSABLE: return float.MaxValue;
				case BlockType.SOFT_BLOCK: return UnitMoved ? float.MaxValue : 0;
				case BlockType.STANDARD: return 0;
				default: throw new Exception(string.Format("No movement cost for {0}", BlockType));
			}
		}
	}
}
