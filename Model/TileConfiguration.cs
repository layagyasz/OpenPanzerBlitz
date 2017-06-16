using System;
namespace PanzerBlitz
{
	public class TileConfiguration
	{
		public static readonly TileConfiguration OPEN = new TileConfiguration("Open", 0, 1, 1, false, false);

		public readonly string Name;
		public readonly int DieModifier;
		public readonly int MoveCost;
		public readonly int TruckMoveCost;
		public readonly bool TreatUnitsAsArmored;
		public readonly bool MustAttackAllUnits;

		public TileConfiguration(
			string Name,
			int DieModifier,
			int MoveCost,
			int TruckMoveCost,
			bool TreatUnitsAsArmored,
			bool MustAttackAllUnits)
		{
			this.Name = Name;
			this.DieModifier = DieModifier;
			this.MoveCost = MoveCost;
			this.TruckMoveCost = TruckMoveCost;
			this.TreatUnitsAsArmored = TreatUnitsAsArmored;
			this.MustAttackAllUnits = MustAttackAllUnits;
		}
	}
}
