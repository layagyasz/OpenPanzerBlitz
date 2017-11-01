using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class LoadOrder : Order
	{
		public readonly Unit Carrier;
		public readonly Unit Passenger;
		public readonly bool UseMovement;

		public Army Army
		{
			get
			{
				return Carrier.Army;
			}
		}

		public LoadOrder(Unit Carrier, Unit Passenger, bool UseMovement = true)
		{
			this.Carrier = Carrier;
			this.Passenger = Passenger;
			this.UseMovement = UseMovement;
		}

		public LoadOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Unit)Objects[Stream.ReadInt32()], (Unit)Objects[Stream.ReadInt32()], Stream.ReadBoolean()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Carrier.Id);
			Stream.Write(Passenger.Id);
			Stream.Write(UseMovement);
		}

		public bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			if (TurnComponent == TurnComponent.DEPLOYMENT) return UseMovement;

			return Carrier.Configuration.IsVehicle
					   ? TurnComponent == TurnComponent.VEHICLE_MOVEMENT
							: TurnComponent == TurnComponent.NON_VEHICLE_MOVEMENT;
		}

		public OrderInvalidReason Validate()
		{
			return Carrier.CanLoad(Passenger);
		}

		public OrderStatus Execute(Random Random)
		{
			if (Validate() == OrderInvalidReason.NONE)
			{
				Carrier.Load(Passenger, UseMovement);
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}
	}
}
