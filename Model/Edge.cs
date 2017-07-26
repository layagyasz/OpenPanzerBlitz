using System;
namespace PanzerBlitz
{
	public class Edge : TileComponent
	{
		public static Edge TOWN =
			new Edge("Town", 1, .5f, .5f, 0, 0, false, false, false, true, true, false, false, false, true);
		public static Edge FOREST =
			new Edge("Forest", 1, 1, 1, 0, 0, false, false, true, false, false, false, false, false, true);
		public static Edge SLOPE =
			new Edge("Slope", 0, 3, 4, 0, 0, false, false, false, false, false, true, false, false, false);
		public static readonly Edge WATER =
			new Edge("Water", 0, 1, 1, 0, 0, false, true, true, false, false, false, false, false, false);

		public static Edge[] EDGES = { null, TOWN, FOREST, SLOPE, WATER };

		public Edge(
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
