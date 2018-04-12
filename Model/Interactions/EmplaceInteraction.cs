namespace PanzerBlitz
{
	public class EmplaceInteraction : InteractionBase
	{
		public int Turns { get; private set; }

		public EmplaceInteraction(Unit Agent, Unit Object)
			: base(Agent, Object) { }

		public override OrderInvalidReason Validate()
		{
			if (!Agent.Configuration.CanClearMines) return OrderInvalidReason.UNIT_NO_ENGINEER;
			if (!Object.Configuration.IsEmplaceable()) return OrderInvalidReason.TARGET_NOT_EMPLACEABLE;
			if (Agent.Position == null
				|| Object.Position == null
				|| Agent.Position.HexCoordinate.Distance(Object.Position.HexCoordinate) > 1)
				return OrderInvalidReason.TARGET_OUT_OF_RANGE;
			return OrderInvalidReason.NONE;
		}

		public override bool Apply(Unit Unit)
		{
			if (Validate() != OrderInvalidReason.NONE) return false;
			return true;
		}

		public void Tick()
		{
			Turns++;
		}
	}
}
