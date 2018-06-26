using System.Linq;

namespace PanzerBlitz
{
	public class FortifyInteraction : InteractionBase
	{
		public int Turns { get; private set; }

		public FortifyInteraction(Unit Agent, Unit Object)
			: base(Agent, Object) { }

		public override OrderInvalidReason Validate()
		{
			if (Object.Configuration.UnitClass != UnitClass.FORT) return OrderInvalidReason.UNIT_NO_FORT;
			if (Agent.Position == null
				|| Object.Position == null
				|| Agent.Position != Object.Position)
				return OrderInvalidReason.TARGET_OUT_OF_RANGE;
			if (Object.Interactions.Where(i => i is FortifyInteraction)
				.Sum(i => i.Master.Configuration.GetStackSize()) >= Object.Army.Configuration.Faction.StackLimit)
				return OrderInvalidReason.UNIT_STACK_LIMIT;
			return OrderInvalidReason.NONE;
		}

		public override bool Apply(Unit Unit)
		{
			if (Validate() != OrderInvalidReason.NONE) return false;
			return true;
		}
	}
}
