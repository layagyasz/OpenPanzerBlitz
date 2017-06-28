﻿using System;
namespace PanzerBlitz
{
	public class TilePathOverlay : TileComponent
	{
		public static readonly TilePathOverlay ROAD =
			new TilePathOverlay("Road", 0, .5f, .5f, false, false, false, false);
		public static readonly TilePathOverlay STREAM =
			new TilePathOverlay("Stream", 0, 3, 5, false, false, false, false);

		public static TilePathOverlay[] PATH_OVERLAYS = { null, ROAD, STREAM };

		public TilePathOverlay(
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
