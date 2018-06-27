using System.Linq;

namespace PanzerBlitz
{
	public class FortifyInteraction : InteractionBase
	{
		public override bool IsWork
		{
			get
			{
				return false;
			}
		}

		public FortifyInteraction(Unit Agent, Unit Object)
			: base(Agent, Object) { }

		public override OrderInvalidReason Validate()
		{
			return OrderInvalidReason.NONE;
		}

		public override bool Apply(Unit Unit)
		{
			if (Validate() != OrderInvalidReason.NONE) return false;
			return true;
		}
	}
}
