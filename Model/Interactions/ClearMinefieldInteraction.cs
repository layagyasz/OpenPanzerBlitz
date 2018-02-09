using System;
using System.Linq;

namespace PanzerBlitz
{
	public class ClearMinefieldInteraction : Interaction
	{
		public readonly Unit Agent;
		public readonly Unit Object;

		public object Master
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
			if (!Agent.Configuration.IsEngineer) return OrderInvalidReason.UNIT_NO_ENGINEER;
			if (Object.Configuration.UnitClass != UnitClass.MINEFIELD) return OrderInvalidReason.ILLEGAL;
			if (Agent.Position == null
				|| Object.Position == null
				|| !Agent.Position.Neighbors().Contains(Object.Position))
				return OrderInvalidReason.TARGET_OUT_OF_RANGE;
			return OrderInvalidReason.NONE;
		}

		public bool Apply()
		{
			if (Validate() != OrderInvalidReason.NONE) return false;
			Object.HandleCombatResult(CombatResult.DISRUPT);
			return true;
		}

		public bool Cancel()
		{
			Agent.CancelInteraction();
			Object.CancelInteraction();
			return true;
		}
	}
}
