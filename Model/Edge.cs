﻿using System;
namespace PanzerBlitz
{
	public class Edge : TileComponent
	{
		public static Edge TOWN = new Edge("Town", 1, .5f, .5f, false, false, true, true);
		public static Edge FOREST = new Edge("Forest", 1, 1, 1, false, true, false, false);
		public static Edge SLOPE = new Edge("Slope", 0, 3, 4, false, false, false, false);

		public static Edge[] EDGES = { null, TOWN, FOREST, SLOPE };

		public Edge(
			string Name,
			int DieModifier,
			float MoveCost,
			float TruckMoveCost,
			bool NoCrossing,
			bool NoVehicleCrossing,
			bool TreatUnitsAsArmored,
			bool MustAttackAllUnits)
			: base(
				Name,
				DieModifier,
				MoveCost,
				TruckMoveCost,
				NoCrossing,
				NoVehicleCrossing,
				TreatUnitsAsArmored,
				MustAttackAllUnits)
		{ }
	}
}
