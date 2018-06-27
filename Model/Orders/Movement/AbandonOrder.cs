using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class AbandonOrder : Order
	{
		public readonly Unit Unit;

		public Army Army
		{
			get
			{
				return Unit.Army;
			}
		}

		public AbandonOrder(Unit Unit)
		{
			this.Unit = Unit;
		}

		public AbandonOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Unit)Objects[Stream.ReadInt32()]) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Unit.Id);
		}

		public bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			if (TurnComponent == TurnComponent.DEPLOYMENT) return true;
			return Unit.Configuration.IsVehicle
					   ? TurnComponent == TurnComponent.VEHICLE_MOVEMENT
						   : TurnComponent == TurnComponent.NON_VEHICLE_MOVEMENT;
		}

		public Order CloneIfStateful()
		{
			return this;
		}

		public OrderInvalidReason Validate()
		{
			return Unit.CanAbandon();
		}

		public OrderStatus Execute(Random Random)
		{
			if (Validate() == OrderInvalidReason.NONE)
			{
				Unit.Abandon();
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}
	}
}
