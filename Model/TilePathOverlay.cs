using System;
namespace PanzerBlitz
{
	public class TilePathOverlay : TileComponent
	{
		public static readonly TilePathOverlay ROAD =
			new TilePathOverlay("Road", 0, .5f, .5f, true, false, false, false, false, false, false, false);
		public static readonly TilePathOverlay STREAM =
			new TilePathOverlay("Stream", 0, 3, 5, false, false, false, false, false, false, true, false);

		public static TilePathOverlay[] PATH_OVERLAYS = { null, ROAD, STREAM };

		public TilePathOverlay(
			string Name,
			int DieModifier,
			float MoveCost,
			float TruckMoveCost,
			bool RoadMove,
			bool NoCrossing,
			bool NoVehicleCrossing,
			bool TreatUnitsAsArmored,
			bool MustAttackAllUnits,
			bool Elevated,
			bool Depressed,
			bool BlocksLineOfSight)
			: base(
				Name,
				DieModifier,
				MoveCost,
				TruckMoveCost,
				RoadMove,
				NoCrossing,
				NoVehicleCrossing,
				TreatUnitsAsArmored,
				MustAttackAllUnits,
				Elevated,
				Depressed,
				BlocksLineOfSight)
		{ }
	}
}
