using System;
namespace PanzerBlitz
{
	public class TileBase : TileComponent
	{
		public static readonly TileBase CLEAR =
			new TileBase("Clear", 0, 1, 1, false, false, false, false);
		public static readonly TileBase SWAMP =
			new TileBase("Swamp", 1, 1, 1, false, true, false, false);
		public static readonly TileBase WATER =
			new TileBase("Water", 0, 1, 1, true, true, false, false);
		public static readonly TileBase SLOPE =
			new TileBase("Slope", 0, 3, 4, false, false, false, false);

		public TileBase(
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
