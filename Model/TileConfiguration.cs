using System;
namespace PanzerBlitz
{
	public class TileConfiguration
	{
		public static readonly TileConfiguration CLEAR =
			new TileConfiguration("Clear", 0, 1, 1, false, false, false, false);
		public static readonly TileConfiguration SWAMP =
			new TileConfiguration("Swamp", 1, 1, 1, false, true, false, false);
		public static readonly TileConfiguration WATER =
			new TileConfiguration("Water", 0, 1, 1, true, true, false, false);
		public static readonly TileConfiguration SLOPE =
			new TileConfiguration("Slope", 0, 3, 4, false, false, false, false);

		public readonly string Name;
		public readonly int DieModifier;
		public readonly float MoveCost;
		public readonly float TruckMoveCost;
		public readonly bool NoCrossing;
		public readonly bool NoVehicleCrossing;
		public readonly bool TreatUnitsAsArmored;
		public readonly bool MustAttackAllUnits;

		public TileConfiguration(
			string Name,
			int DieModifier,
			int MoveCost,
			int TruckMoveCost,
			bool NoCrossing,
			bool NoVehicleCrossing,
			bool TreatUnitsAsArmored,
			bool MustAttackAllUnits)
		{
			this.Name = Name;
			this.DieModifier = DieModifier;
			this.MoveCost = MoveCost;
			this.TruckMoveCost = TruckMoveCost;
			this.NoCrossing = NoCrossing;
			this.NoVehicleCrossing = NoVehicleCrossing;
			this.TreatUnitsAsArmored = TreatUnitsAsArmored;
			this.MustAttackAllUnits = MustAttackAllUnits;
		}
	}
}
