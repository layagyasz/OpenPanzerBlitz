namespace PanzerBlitz
{
	public struct EmplacementRequirements
	{
		public static readonly EmplacementRequirements TRUCK_BRIDGE = new EmplacementRequirements(10, 2, 2, false);
		public static readonly EmplacementRequirements TANK_BRIDGE = new EmplacementRequirements(15, 4, 4, false);
		public static readonly EmplacementRequirements MINEFIELD = new EmplacementRequirements(16, 1, 4, true);

		public static EmplacementRequirements GetEmplacementRequirementsFor(Unit Unit)
		{
			if (Unit.Configuration.UnitClass == UnitClass.BRIDGE)
				return Unit.Configuration.CanSupportArmored ? TANK_BRIDGE : TRUCK_BRIDGE;
			return MINEFIELD;
		}

		public readonly int Turns;
		public readonly int MinUnits;
		public readonly int MaxUnits;
		public readonly bool PoolTurns;

		public EmplacementRequirements(int Turns, int MinUnits, int MaxUnits, bool PoolTurns)
		{
			this.Turns = Turns;
			this.MinUnits = MinUnits;
			this.MaxUnits = MaxUnits;
			this.PoolTurns = PoolTurns;
		}
	}
}
