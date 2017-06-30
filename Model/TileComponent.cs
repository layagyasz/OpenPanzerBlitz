using System;
namespace PanzerBlitz
{
	public class TileComponent
	{
		public readonly string Name;
		public readonly int DieModifier;
		public readonly float MoveCost;
		public readonly float TruckMoveCost;
		public readonly bool RoadMove;
		public readonly bool NoCrossing;
		public readonly bool NoVehicleCrossing;
		public readonly bool TreatUnitsAsArmored;
		public readonly bool MustAttackAllUnits;

		public TileComponent(
			string Name,
			int DieModifier,
			float MoveCost,
			float TruckMoveCost,
			bool RoadMove,
			bool NoCrossing,
			bool NoVehicleCrossing,
			bool TreatUnitsAsArmored,
			bool MustAttackAllUnits)
		{
			this.Name = Name;
			this.DieModifier = DieModifier;
			this.MoveCost = MoveCost;
			this.TruckMoveCost = TruckMoveCost;
			this.RoadMove = RoadMove;
			this.NoCrossing = NoCrossing;
			this.NoVehicleCrossing = NoVehicleCrossing;
			this.TreatUnitsAsArmored = TreatUnitsAsArmored;
			this.MustAttackAllUnits = MustAttackAllUnits;
		}
	}
}
