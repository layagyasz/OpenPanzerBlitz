using System;
namespace PanzerBlitz
{
	public class LoadOrder : Order
	{
		public readonly Unit Carrier;
		public readonly Unit Passenger;

		public LoadOrder(Unit Carrier, Unit Passenger)
		{
			this.Carrier = Carrier;
			this.Passenger = Passenger;
		}

		public NoLoadReason Validate()
		{
			return Carrier.CanLoad(Passenger);
		}

		public bool Execute(Random Random)
		{
			if (Validate() == NoLoadReason.NONE)
			{
				Carrier.Load(Passenger);
				return true;
			}
			else return false;
		}
	}
}
