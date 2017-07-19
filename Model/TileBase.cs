﻿using System;
namespace PanzerBlitz
{
	public class TileBase : TileComponent
	{
		public static readonly TileBase CLEAR =
			new TileBase("Clear", 0, 1, 1, false, false, false, false, false, false, false, false);
		public static readonly TileBase SWAMP =
			new TileBase("Swamp", 1, 1, 1, false, false, true, false, false, false, false, false);
		public static readonly TileBase WATER =
			new TileBase("Water", 0, 1, 1, false, true, true, false, false, false, false, false);
		public static readonly TileBase SLOPE =
			new TileBase("Slope", 0, 3, 4, false, false, false, false, false, true, false, false);
		public static readonly TileBase STREAM_FORD =
			new TileBase("Stream Ford", 0, 1, 1, false, false, false, false, false, false, false, false);

		public static readonly TileBase[] TILE_BASES = { CLEAR, SWAMP, WATER, SLOPE, STREAM_FORD };

		public TileBase(
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
