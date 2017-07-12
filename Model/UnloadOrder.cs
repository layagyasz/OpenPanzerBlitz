using System;
namespace PanzerBlitz
{
	public class UnloadOrder : Order
	{
		public readonly Unit Carrier;

		public UnloadOrder(Unit Carrier)
		{
			this.Carrier = Carrier;
		}

		public NoUnloadReason Validate()
		{
			return Carrier.CanUnload();
		}

		public bool Execute(Random Random)
		{
			if (Validate() == NoUnloadReason.NONE)
			{
				Carrier.Unload();
				return true;
			}
			else return false;
		}
	}
}
