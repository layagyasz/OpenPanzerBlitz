using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class MountOrder : Order
	{
		public readonly Unit Unit;
		public readonly bool UseMovement;

		public Army Army
		{
			get
			{
				return Unit.Army;
			}
		}

		public MountOrder(Unit Unit, bool UseMovement = true)
		{
			this.Unit = Unit;
			this.UseMovement = UseMovement;
		}

		public MountOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Unit)Objects[Stream.ReadInt32()], Stream.ReadBoolean()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Unit.Id);
			Stream.Write(UseMovement);
		}

		public bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			if (UseMovement)
			{
				return Unit.Configuration.IsVehicle
					   ? TurnComponent == TurnComponent.VEHICLE_MOVEMENT
							: TurnComponent == TurnComponent.NON_VEHICLE_MOVEMENT;
			}
			return TurnComponent == TurnComponent.DEPLOYMENT;
		}

		public OrderInvalidReason Validate()
		{
			return Unit.CanMount(!UseMovement);
		}

		public OrderStatus Execute(Random Random)
		{
			if (Validate() == OrderInvalidReason.NONE)
			{
				Unit.Mount(UseMovement);
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}
	}
}
