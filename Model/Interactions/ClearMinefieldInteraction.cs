using System.Linq;

namespace PanzerBlitz
{
	public class ClearMinefieldInteraction : Interaction
	{
		public readonly Unit Agent;
		public readonly Unit Object;

		public Unit Master
		{
			get
			{
				return Agent;
			}
		}

		public ClearMinefieldInteraction(Unit Agent, Unit Object)
		{
			this.Agent = Agent;
			this.Object = Object;
		}

		public OrderInvalidReason Validate()
		{
			if (!Agent.Configuration.CanClearMines) return OrderInvalidReason.UNIT_NO_ENGINEER;
			if (Object.Configuration.UnitClass != UnitClass.MINEFIELD) return OrderInvalidReason.ILLEGAL;
			if (Agent.Position == null
				|| Object.Position == null
				|| (Agent.Position != Object.Position && !Agent.Position.Neighbors().Contains(Object.Position)))
				return OrderInvalidReason.TARGET_OUT_OF_RANGE;
			return OrderInvalidReason.NONE;
		}

		public bool Apply(Unit Unit)
		{
			if (Validate() != OrderInvalidReason.NONE) return false;
			if (Unit == Object) Unit.HandleCombatResult(CombatResult.DISRUPT);
			return true;
		}

		public bool Cancel()
		{
			Agent.CancelInteraction(this);
			Object.CancelInteraction(this);
			return true;
		}

		public override string ToString()
		{
			return string.Format("[ClearMinefieldInteraction: Agent={0}, Object={1}]", Agent, Object);
		}
	}
}
