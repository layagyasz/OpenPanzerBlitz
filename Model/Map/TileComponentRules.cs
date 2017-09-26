using System;
namespace PanzerBlitz
{
	public class TileComponentRules
	{
		public static readonly TileComponentRules BASE_CLEAR =
			new TileComponentRules(0, 1, 1, 0, 0, false, false, false, false, false, false, false, false, false, false);
		public static readonly TileComponentRules BASE_SWAMP =
			new TileComponentRules(1, 1, 1, 0, 0, false, false, true, false, false, false, false, false, false, false);
		public static readonly TileComponentRules BASE_SLOPE =
			new TileComponentRules(0, 3, 4, 0, 0, false, false, false, false, false, true, false, false, false, false);

		public static readonly TileComponentRules EDGE_TOWN =
			new TileComponentRules(1, .5f, .5f, 0, 0, false, false, false, true, true, false, false, false, true, true);
		public static readonly TileComponentRules EDGE_FOREST =
			new TileComponentRules(1, 1, 1, 0, 0, false, false, true, false, false, false, false, false, true, true);
		public static readonly TileComponentRules EDGE_SLOPE =
			new TileComponentRules(0, 3, 4, 0, 0, false, false, false, false, false, true, false, false, false, false);
		public static readonly TileComponentRules EDGE_WATER =
			new TileComponentRules(0, 1, 1, 0, 0, false, true, true, false, false, false, false, false, false, false);

		public static readonly TileComponentRules PATH_ROAD = new TileComponentRules(
			0, .5f, .5f, 0, 0, true, false, false, false, false, false, false, false, false, false);
		public static readonly TileComponentRules PATH_STREAM = new TileComponentRules(
			0, 0, 0, 3, 5, false, false, false, false, false, false, true, false, false, false);
		public static readonly TileComponentRules PATH_STREAM_FORD = new TileComponentRules(
			0, 0, 0, 0, 0, false, false, false, false, false, false, false, true, false, false);

		public readonly int DieModifier;
		public readonly float MoveCost;
		public readonly float TruckMoveCost;
		public readonly float LeaveCost;
		public readonly float TruckLeaveCost;
		public readonly bool RoadMove;
		public readonly bool NoCrossing;
		public readonly bool NoVehicleCrossing;
		public readonly bool TreatUnitsAsArmored;
		public readonly bool MustAttackAllUnits;
		public readonly bool Elevated;
		public readonly bool Depressed;
		public readonly bool DepressedTransition;
		public readonly bool BlocksLineOfSight;
		public readonly bool Concealing;

		public TileComponentRules(
			int DieModifier,
			float MoveCost,
			float TruckMoveCost,
			float LeaveCost,
			float TruckLeaveCost,
			bool RoadMove,
			bool NoCrossing,
			bool NoVehicleCrossing,
			bool TreatUnitsAsArmored,
			bool MustAttackAllUnits,
			bool Elevated,
			bool Depressed,
			bool DepressedTransition,
			bool BlocksLineOfSight,
			bool Concealing)
		{
			this.DieModifier = DieModifier;
			this.MoveCost = MoveCost;
			this.TruckMoveCost = TruckMoveCost;
			this.LeaveCost = LeaveCost;
			this.TruckLeaveCost = TruckLeaveCost;
			this.RoadMove = RoadMove;
			this.NoCrossing = NoCrossing;
			this.NoVehicleCrossing = NoVehicleCrossing;
			this.TreatUnitsAsArmored = TreatUnitsAsArmored;
			this.MustAttackAllUnits = MustAttackAllUnits;
			this.Elevated = Elevated;
			this.Depressed = Depressed;
			this.DepressedTransition = DepressedTransition;
			this.BlocksLineOfSight = BlocksLineOfSight;
			this.Concealing = Concealing;
		}
	}
}
