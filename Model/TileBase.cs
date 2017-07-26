using System;
namespace PanzerBlitz
{
	public class TileBase : TileComponent
	{
		public static readonly TileBase CLEAR =
			new TileBase("Clear", 0, 1, 1, 0, 0, false, false, false, false, false, false, false, false, false);
		public static readonly TileBase SWAMP =
			new TileBase("Swamp", 1, 1, 1, 0, 0, false, false, true, false, false, false, false, false, false);
		public static readonly TileBase SLOPE =
			new TileBase("Slope", 0, 3, 4, 0, 0, false, false, false, false, false, true, false, false, false);

		public static readonly TileBase[] TILE_BASES = { CLEAR, SWAMP, SLOPE };

		public TileBase(
			string Name,
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
			bool BlocksLineOfSight)
			: base(
				Name,
				DieModifier,
				MoveCost,
				TruckMoveCost,
				LeaveCost,
				TruckLeaveCost,
				RoadMove,
				NoCrossing,
				NoVehicleCrossing,
				TreatUnitsAsArmored,
				MustAttackAllUnits,
				Elevated,
				Depressed,
				DepressedTransition,
				BlocksLineOfSight)
		{ }
	}
}
