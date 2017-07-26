using System;
namespace PanzerBlitz
{
	public class TileComponent
	{
		public readonly string Name;
		public readonly int DieModifier;
		public readonly float MoveCost;
		public readonly float TruckMoveCost;
		public readonly float LeaveCost;
		public readonly float TruckLeaveCost;
		public readonly bool RoadMove;
		public readonly bool NoCrossing;
		public readonly bool NoVehicleCrossing;
		public readonly bool TreatUnitsAsArmored;
		public readonly bool MustAttackAllUnits;
		public readonly bool Elevated;
		public readonly bool Depressed;
		public readonly bool DepressedTransition;
		public readonly bool BlocksLineOfSight;

		public TileComponent(
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
		{
			this.Name = Name;
			this.DieModifier = DieModifier;
			this.MoveCost = MoveCost;
			this.TruckMoveCost = TruckMoveCost;
			this.LeaveCost = LeaveCost;
			this.TruckLeaveCost = TruckLeaveCost;
			this.RoadMove = RoadMove;
			this.NoCrossing = NoCrossing;
			this.NoVehicleCrossing = NoVehicleCrossing;
			this.TreatUnitsAsArmored = TreatUnitsAsArmored;
			this.MustAttackAllUnits = MustAttackAllUnits;
			this.Elevated = Elevated;
			this.Depressed = Depressed;
			this.DepressedTransition = DepressedTransition;
			this.BlocksLineOfSight = BlocksLineOfSight;
		}

		public override string ToString()
		{
			return string.Format("[TileComponent Name={0}]", Name);
		}
	}
}
