using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class UnloadOrder : Order
	{
		public readonly Unit Carrier;
		public readonly bool UseMovement;

		public Army Army
		{
			get
			{
				return Carrier.Army;
			}
		}

		public UnloadOrder(Unit Carrier, bool UseMovement = true)
		{
			this.Carrier = Carrier;
			this.UseMovement = UseMovement;
		}

		public UnloadOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Unit)Objects[Stream.ReadInt32()], Stream.ReadBoolean()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Carrier.Id);
			Stream.Write(UseMovement);
		}

		public bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			if (UseMovement)
			{
				return Carrier.Configuration.IsVehicle
					   ? TurnComponent == TurnComponent.VEHICLE_MOVEMENT
							: TurnComponent == TurnComponent.NON_VEHICLE_MOVEMENT;
			}
			return TurnComponent == TurnComponent.DEPLOYMENT;
		}

		public Order CloneIfStateful()
		{
			return this;
		}

		public OrderInvalidReason Validate()
		{
			return Carrier.CanUnload(UseMovement);
		}

		public OrderStatus Execute(Random Random)
		{
			if (Validate() == OrderInvalidReason.NONE)
			{
				Carrier.Unload(UseMovement);
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}

		public override string ToString()
		{
			return string.Format(
				"[UnloadOrder: Carrier={0}, Passenger={1}, UseMovement={2}]", Carrier, Carrier.Passenger, UseMovement);
		}
	}
}
