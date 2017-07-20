using System;
namespace PanzerBlitz
{
	public class UnloadOrder : Order
	{
		public readonly Unit Carrier;
		public readonly bool UseMovement;

		public UnloadOrder(Unit Carrier, bool UseMovement = true)
		{
			this.Carrier = Carrier;
			this.UseMovement = UseMovement;
		}

		public NoUnloadReason Validate()
		{
			return Carrier.CanUnload();
		}

		public bool Execute(Random Random)
		{
			if (Validate() == NoUnloadReason.NONE)
			{
				Carrier.Unload(UseMovement);
				return true;
			}
			else return false;
		}
	}
}
