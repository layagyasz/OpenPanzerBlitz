using System;
namespace PanzerBlitz
{
	public class TilePathOverlay : TileComponent
	{
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
