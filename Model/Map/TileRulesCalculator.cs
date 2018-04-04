using System;
using System.Linq;

namespace PanzerBlitz
{
	public class TileRulesCalculator
	{
		static readonly TerrainAttribute[] LEAVING_DEPRESSED_ATTRIBUTES =
		{
			TerrainAttribute.SLOPED,
			TerrainAttribute.ROUGH,
			TerrainAttribute.UP_HILL
		};

		public readonly Tile Tile;

		public bool MustAttackAllUnits { get; private set; }
		public bool TreatUnitsAsArmored { get; private set; }
		public bool Depressed { get; private set; }
		public bool DepressedTransition { get; private set; }
		public bool Concealing { get; private set; }
		public bool LowProfileConcealing { get; private set; }
		public bool Watery { get; private set; }
		public bool Bridged { get; private set; }
		public bool Bridgeable { get; private set; }
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
			return GetMoveCost(Unit, To, RoadMovement, IgnoreOccupyingUnits).UnableReason == OrderInvalidReason.NONE;
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
			Depressed = Tile.GetBaseRules().HasAttribute(TerrainAttribute.DEPRESSED)
							 || Tile.GetEdgeRules().Any(i => i != null && i.HasAttribute(TerrainAttribute.DEPRESSED))
							 || Tile.GetPathOverlayRules().Any(
								i => i != null && i.HasAttribute(TerrainAttribute.DEPRESSED));
			DepressedTransition = Tile.GetBaseRules().DepressedTransition
									   || Tile.GetEdgeRules().Any(i => i != null && i.DepressedTransition)
									   || Tile.GetPathOverlayRules().Any(i => i != null && i.DepressedTransition);
			Concealing = Tile.GetBaseRules().Concealing
							  || Tile.GetEdgeRules().Any(i => i != null && i.Concealing)
							  || Tile.GetPathOverlayRules().Any(i => i != null && i.Concealing);
			LowProfileConcealing = Tile.GetBaseRules().LowProfileConcealing
							  || Tile.GetEdgeRules().Any(i => i != null && i.LowProfileConcealing)
							  || Tile.GetPathOverlayRules().Any(i => i != null && i.LowProfileConcealing);
			Watery = Tile.GetBaseRules().HasAttribute(TerrainAttribute.WATER)
					 	|| Tile.GetBaseRules().HasAttribute(TerrainAttribute.SWAMP)
						|| Tile.GetEdgeRules().All(
							 i => i != null
								 && (i.HasAttribute(TerrainAttribute.WATER) || i.HasAttribute(TerrainAttribute.SWAMP)))
					 	|| Tile.GetPathOverlayRules().Any(
							 i => i != null
								 && (i.HasAttribute(TerrainAttribute.WATER) || i.HasAttribute(TerrainAttribute.SWAMP)));

			var lowerPaths = Tile.GetPathOverlayRules()
									 .Where(i => i != null)
									 .Select(i => i.HasAttribute(TerrainAttribute.WATER)
											 || i.HasAttribute(TerrainAttribute.DEPRESSED)
											 || i.DepressedTransition)
									 .ToArray();
			int diffCount = 0;
			for (int i = 0; i < lowerPaths.Length; ++i)
			{
				if (lowerPaths[i] != lowerPaths[(i + 1) % lowerPaths.Length]) diffCount++;
			}
			Bridged = diffCount > 3;
			if (!Bridged)
			{
				for (int i = 0; i < 6; ++i)
				{
					var edge = Tile.GetEdgeRules(i);
					var path = Tile.GetPathOverlayRules(i);
					if (edge == null || path == null) continue;
					Bridged |= !path.HasAttribute(TerrainAttribute.DEPRESSED)
									&& !path.HasAttribute(TerrainAttribute.WATER)
									&& (edge.HasAttribute(TerrainAttribute.DEPRESSED)
										|| edge.HasAttribute(TerrainAttribute.WATER));
				}
			}
			Bridgeable = !Bridged
				&& Tile.GetPathOverlayRules().Any(i => i != null && i.HasAttribute(TerrainAttribute.DEPRESSED));

			DieModifier =
				Math.Max(
					Tile.GetBaseRules().DieModifier,
					Math.Max(
						Tile.GetEdgeRules().Max(i => i == null ? 0 : i.DieModifier),
						Tile.GetPathOverlayRules().Max(i => i == null ? 0 : i.DieModifier)));

			bool elevated = Tile.GetBaseRules().HasAttribute(TerrainAttribute.SLOPED)
							 || Tile.GetEdgeRules().Any(i => i != null && i.HasAttribute(TerrainAttribute.SLOPED))
							 || Tile.GetPathOverlayRules().Any(
									i => i != null && i.HasAttribute(TerrainAttribute.SLOPED));
			TieredElevation = Tile.Configuration.Elevation + (elevated ? 1 : 0);
			SubTieredElevation = 2 * Tile.Configuration.Elevation + (elevated ? 1 : 0);
		}

		public MovementCost GetMoveCost(Unit Unit, Tile To, bool RoadMovement, bool IgnoreOccupyingUnits = false)
		{
			if (!IgnoreOccupyingUnits && Unit.CanEnter(To) == OrderInvalidReason.TILE_ENEMY_OCCUPIED)
				return new MovementCost(OrderInvalidReason.TILE_ENEMY_OCCUPIED);

			var toBlock = To.GetUnitBlockType();
			var fromBlock = Tile.GetUnitBlockType();

			bool unitMoved = Unit.Moved || !Tile.Units.Contains(Unit);
			bool adjacent = !unitMoved && !Unit.Moved && To.NeighborTiles.Any(i => i != null && i.Units.Contains(Unit));

			if (toBlock == BlockType.HARD_BLOCK && !adjacent)
				return new MovementCost(OrderInvalidReason.UNIT_NO_MOVE);
			if ((fromBlock == BlockType.HARD_BLOCK || fromBlock == BlockType.SOFT_BLOCK) && unitMoved)
				return new MovementCost(OrderInvalidReason.UNIT_NO_MOVE);

			bool useRoadMovement = RoadMovement
							&& !Tile.Map.Environment.IsRoadMovementRestricted(Unit.Configuration.UnitClass)
							&& !Unit.Configuration.MovementRules.CannotUseRoadMovement
							&& !(toBlock == BlockType.STANDARD && (!To.Units.Contains(Unit) || To.Units.Count() > 1))
							&& !(fromBlock == BlockType.STANDARD
								 && (!Tile.Units.Contains(Unit) || Tile.Units.Count() > 1))
							&& !(Unit.Configuration.CannotUseRoadMovementWithOversizedPassenger
								 && Unit.Passenger != null
								 && Unit.Passenger.Configuration.IsOversizedPassenger);

			var edge = Tile.GetEdgeRules(To);
			var path = Tile.GetPathOverlayRules(To);

			bool roaded = path != null && path.RoadMove;

			var fromBridged = Tile.Units.Any(
				i => i.Configuration.UnitClass == UnitClass.BRIDGE
				&& i.Emplaced
				&& (!Unit.Configuration.IsArmored || i.Configuration.CanSupportArmored));
			var toBridged = Tile.Units.Any(
				i => i.Configuration.UnitClass == UnitClass.BRIDGE
				&& i.Emplaced
				&& (!Unit.Configuration.IsArmored || i.Configuration.CanSupportArmored));

			bool leavingDepressed = Tile.Rules.Depressed
										&& (path == null || !path.HasAttribute(TerrainAttribute.DEPRESSED))
										&& (path == null || !path.DepressedTransition)
										&& !roaded;

			UnitMovementRules movementRules = Unit.Configuration.MovementRules;
			var leaveCost = new MovementCost(0f);
			if (leavingDepressed && !fromBridged)
			{
				leaveCost = 1 + LEAVING_DEPRESSED_ATTRIBUTES
					.Select(i => movementRules[i].GetMoveCost(adjacent, unitMoved))
					.Aggregate((i, j) => i + j);
			}

			var crossCost = GetRulesMoveCost(
				edge, movementRules, adjacent, unitMoved, roaded, useRoadMovement, Unit);

			var enterCost = new MovementCost(0f);
			if (!toBridged)
			{
				enterCost = GetRulesMoveCost(
					To.GetBaseRules(), movementRules, adjacent, unitMoved, roaded, useRoadMovement, Unit);

				var edgeCost = new MovementCost(0f);
				var intersectCost = new MovementCost(0f);
				for (int i = 0; i < 6; ++i)
				{
					var eMove = GetRulesMoveCost(
						To.GetEdgeRules(i), movementRules, adjacent, unitMoved, roaded, useRoadMovement, Unit);
					if (eMove.IsSet())
					{
						if (edgeCost.IsSet()) edgeCost = MovementCost.Min(edgeCost, eMove);
						else edgeCost = eMove;
					}

					var pMove = GetRulesMoveCost(
						To.GetPathOverlayRules(i), movementRules, adjacent, unitMoved, roaded, useRoadMovement, Unit);
					if (pMove.IsSet())
					{
						if (intersectCost.IsSet()) intersectCost = MovementCost.Min(intersectCost, pMove);
						else intersectCost = pMove;
					}
				}

				enterCost = (edgeCost.IsSet() ? MovementCost.Min(edgeCost, enterCost) : enterCost) + intersectCost;
			}

			if (path != null && path.OverrideBaseMovement && (!path.RoadMove || useRoadMovement))
			{
				enterCost = GetRulesMoveCost(
					path, movementRules, adjacent, unitMoved, roaded, useRoadMovement, Unit);
				crossCost = new MovementCost(0f);
			}
			if (Tile.Rules.TieredElevation < To.Rules.TieredElevation)
				enterCost += movementRules[TerrainAttribute.UP_HILL].GetMoveCost(adjacent, unitMoved);
			if (Tile.Configuration.Elevation > To.Configuration.Elevation)
				enterCost += movementRules[TerrainAttribute.DOWN_HILL].GetMoveCost(adjacent, unitMoved);

			float multiplier = Unit.Passenger != null && Unit.Passenger.Configuration.IsOversizedPassenger
								   ? Unit.Configuration.OversizedPassengerMovementMultiplier : 1;

			return multiplier * (1 + enterCost + leaveCost + crossCost);
		}

		MovementCost GetRulesMoveCost(
			TileComponentRules TileRules,
			UnitMovementRules MovementRules,
			bool Adjacent,
			bool UnitMoved,
			bool Road,
			bool UseRoad,
			Unit Unit)
		{
			if (TileRules == null) return new MovementCost(0f);

			var cost = TileRules.GetAttributes()
							 	.Where(i => i == TerrainAttribute.ROADED ? UseRoad : !Road)
					 			.Select(i => MovementRules[i].GetMoveCost(Adjacent, UnitMoved))
								.Aggregate((i, j) => i + j);

			if (!Unit.Configuration.CanCarryInWater
				&& !Road
				&& (TileRules.HasAttribute(TerrainAttribute.WATER) || TileRules.HasAttribute(TerrainAttribute.SWAMP)))
				cost = new MovementCost(OrderInvalidReason.UNIT_NO_CARRY_IN_WATER);

			if (!TileRules.OverrideBaseMovement && cost.UnableReason == OrderInvalidReason.NONE)
				return new MovementCost(0f);
			return cost;
		}
	}
}
