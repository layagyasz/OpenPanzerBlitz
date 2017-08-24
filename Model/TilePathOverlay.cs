using System;
namespace PanzerBlitz
{
	public class TilePathOverlay : TileComponent
	{
		public static readonly TilePathOverlay ROAD = new TilePathOverlay(
			"Road", 0, .5f, .5f, 0, 0, true, false, false, false, false, false, false, false, false, false);
		public static readonly TilePathOverlay STREAM = new TilePathOverlay(
			"Stream", 0, 0, 0, 3, 5, false, false, false, false, false, false, true, false, false, false);
		public static readonly TilePathOverlay STREAM_FORD = new TilePathOverlay(
			"Stream Ford", 0, 0, 0, 0, 0, false, false, false, false, false, false, false, true, false, false);

		public static TilePathOverlay[] PATH_OVERLAYS = { null, ROAD, STREAM, STREAM_FORD };

		public TilePathOverlay(
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
			bool BlocksLineOfSight,
			bool Concealing)
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
				BlocksLineOfSight,
				Concealing)
		{ }
	}
}
