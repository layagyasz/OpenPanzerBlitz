using System;
using System.Linq;

namespace PanzerBlitz
{
	public class TileRulesCalculator
	{
		public readonly Tile Tile;

		public bool MustAttackAllUnits { get; private set; }
		public bool TreatUnitsAsArmored { get; private set; }
		public bool Depressed { get; private set; }
		public bool DepressedTransition { get; private set; }
		public bool Concealing { get; private set; }
		public bool LowProfileConcealing { get; private set; }
		public bool Water { get; private set; }
		public int DieModifier { get; private set; }
		public int TieredElevation { get; private set; }
		public int SubTieredElevation { get; private set; }

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

			bool unitMoved = Unit.Moved || !Tile.Units.Contains(Unit);
			bool adjacent = !unitMoved && !Unit.Moved && To.NeighborTiles.Any(i => i != null && i.Units.Contains(Unit));

			if (toBlock == BlockType.HARD_BLOCK && !adjacent)
				return float.MaxValue;
			if ((fromBlock == BlockType.HARD_BLOCK || fromBlock == BlockType.SOFT_BLOCK) && unitMoved)
				return float.MaxValue;

			bool useRoadMovement = RoadMovement
				&& !Tile.Map.Environment.IsRoadMovementRestricted(Unit.Configuration.UnitClass)
						&& !Unit.Configuration.MovementRules.CannotUseRoadMovement;
			if (toBlock == BlockType.STANDARD && (!To.Units.Contains(Unit) || To.Units.Count() > 1))
				useRoadMovement = false;
			if (fromBlock == BlockType.STANDARD && (!Tile.Units.Contains(Unit) || Tile.Units.Count() > 1))
				useRoadMovement = false;
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

			MustAttackAllUnits = Tile.GetBaseRules().MustAttackAllUnits
									  || Tile.GetEdgeRules().Any(i => i != null && i.MustAttackAllUnits)
									  || Tile.GetPathOverlayRules().Any(i => i != null && i.MustAttackAllUnits);
			TreatUnitsAsArmored = Tile.GetBaseRules().TreatUnitsAsArmored
									   || Tile.GetEdgeRules().Any(i => i != null && i.TreatUnitsAsArmored)
									   || Tile.GetPathOverlayRules().Any(i => i != null && i.TreatUnitsAsArmored);
			Depressed = Tile.GetBaseRules().Depressed
							 || Tile.GetEdgeRules().Any(i => i != null && i.Depressed)
							 || Tile.GetPathOverlayRules().Any(i => i != null && i.Depressed);
			DepressedTransition = Tile.GetBaseRules().DepressedTransition
									   || Tile.GetEdgeRules().Any(i => i != null && i.DepressedTransition)
									   || Tile.GetPathOverlayRules().Any(i => i != null && i.DepressedTransition);
			Concealing = Tile.GetBaseRules().Concealing
							  || Tile.GetEdgeRules().Any(i => i != null && i.Concealing)
							  || Tile.GetPathOverlayRules().Any(i => i != null && i.Concealing);
			LowProfileConcealing = Tile.GetBaseRules().LowProfileConcealing
							  || Tile.GetEdgeRules().Any(i => i != null && i.LowProfileConcealing)
							  || Tile.GetPathOverlayRules().Any(i => i != null && i.LowProfileConcealing);
			Water = Tile.GetBaseRules().Water
						|| Tile.GetBaseRules().Swamp
						|| Tile.GetEdgeRules().All(i => i != null && (i.Water || i.Swamp));

			DieModifier =
				Math.Max(
					Tile.GetBaseRules().DieModifier,
					Math.Max(
						Tile.GetEdgeRules().Max(i => i == null ? 0 : i.DieModifier),
						Tile.GetPathOverlayRules().Max(i => i == null ? 0 : i.DieModifier)));

			bool elevated = Tile.GetBaseRules().Elevated
							 || Tile.GetEdgeRules().Any(i => i != null && i.Elevated)
							 || Tile.GetPathOverlayRules().Any(i => i != null && i.Elevated);
			TieredElevation = Tile.Configuration.Elevation + (elevated ? 1 : 0);
			SubTieredElevation = 2 * Tile.Configuration.Elevation + (elevated ? 1 : 0);
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
				To.GetBaseRules(), MovementRules, Adjacent, UnitMoved, roaded, CanUseRoadMovement);

			float edgeCost = 0;
			float intersectCost = 0;
			for (int i = 0; i < 6; ++i)
			{
				float eMove = GetRulesMoveCost(
					To.GetEdgeRules(i), MovementRules, Adjacent, UnitMoved, roaded, CanUseRoadMovement);
				if (eMove > 0)
				{
					if (edgeCost > 0) edgeCost = Math.Min(edgeCost, eMove);
					else edgeCost = eMove;
				}

				float pMove = GetRulesMoveCost(
					To.GetPathOverlayRules(i), MovementRules, Adjacent, UnitMoved, roaded, CanUseRoadMovement);
				if (pMove > 0)
				{
					if (intersectCost > 0) intersectCost = Math.Min(intersectCost, pMove);
					else intersectCost = pMove;
				}
			}

			enterCost = (edgeCost > 0 ? Math.Min(edgeCost, enterCost) : enterCost) + intersectCost;
			if (Path != null && Path.OverrideBaseMovement && (!Path.RoadMove || CanUseRoadMovement))
			{
				enterCost = GetRulesMoveCost(
					Path, MovementRules, Adjacent, UnitMoved, roaded, CanUseRoadMovement);
				crossCost = 0;
			}
			if (From.RulesCalculator.TieredElevation < To.RulesCalculator.TieredElevation)
				enterCost += GetMoveCost(MovementRules.Uphill, Adjacent, UnitMoved);
			if (From.Configuration.Elevation > To.Configuration.Elevation)
				enterCost += GetMoveCost(MovementRules.Downhill, Adjacent, UnitMoved);

			return 1 + enterCost + leaveCost + crossCost;
		}

		float GetRulesMoveCost(
			TileComponentRules TileRules,
			UnitMovementRules MovementRules,
			bool Adjacent,
			bool UnitMoved,
			bool Roaded,
			bool UseRoad,
			bool CrossesEdge = false)
		{
			if (TileRules == null) return 0;

			float cost = 0;

			if (TileRules.DenseEdge && !Roaded && CrossesEdge)
				cost += GetMoveCost(MovementRules.DenseEdge, Adjacent, UnitMoved);
			if (TileRules.Depressed && !Roaded)
				cost += GetMoveCost(MovementRules.Depressed, Adjacent, UnitMoved);
			if (TileRules.Elevated && !Roaded) cost += GetMoveCost(MovementRules.Sloped, Adjacent, UnitMoved);
			if (TileRules.Frozen && !Roaded) cost += GetMoveCost(MovementRules.Frozen, Adjacent, UnitMoved);
			if (TileRules.Loose && !Roaded) cost += GetMoveCost(MovementRules.Loose, Adjacent, UnitMoved);
			if (TileRules.Roaded && UseRoad) cost += GetMoveCost(MovementRules.Roaded, Adjacent, UnitMoved);
			if (TileRules.Rough && !Roaded) cost += GetMoveCost(MovementRules.Rough, Adjacent, UnitMoved);
			if (TileRules.Swamp && !Roaded) cost += GetMoveCost(MovementRules.Swamp, Adjacent, UnitMoved);
			if (TileRules.Water && !Roaded) cost += GetMoveCost(MovementRules.Water, Adjacent, UnitMoved);

			if (!TileRules.OverrideBaseMovement && cost < float.MaxValue) return 0;
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
