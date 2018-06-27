namespace PanzerBlitz
{
	public class ClearMinefieldInteraction : InteractionBase
	{
		public override bool IsWork
		{
			get
			{
				return true;
			}
		}

		public ClearMinefieldInteraction(Unit Agent, Unit Object)
			: base(Agent, Object) { }

		public override OrderInvalidReason Validate()
		{
			if (!Agent.Configuration.CanClearMines) return OrderInvalidReason.UNIT_NO_ENGINEER;
			if (Object.Configuration.UnitClass != UnitClass.MINEFIELD) return OrderInvalidReason.TARGET_IMMUNE;
			if (!Object.Emplaced) return OrderInvalidReason.TARGET_IMMUNE;
			if (Agent.Position == null
				|| Object.Position == null
				|| Agent.Position.HexCoordinate.Distance(Object.Position.HexCoordinate) > 1)
				return OrderInvalidReason.TARGET_OUT_OF_RANGE;
			return OrderInvalidReason.NONE;
		}

		public override bool Apply(Unit Unit)
		{
			if (Validate() != OrderInvalidReason.NONE) return false;
			if (Unit == Object) Unit.HandleCombatResult(CombatResult.DISRUPT, AttackMethod.NONE, null);
			return true;
		}
	}
}
