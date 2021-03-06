﻿using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class DismountOrder : Order
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

		public DismountOrder(Unit Unit, bool UseMovement = true)
		{
			this.Unit = Unit;
			this.UseMovement = UseMovement;
		}

		public DismountOrder(SerializationInputStream Stream, List<GameObject> Objects)
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

		public Order CloneIfStateful()
		{
			return this;
		}

		public OrderInvalidReason Validate()
		{
			return Unit.CanDismount();
		}

		public OrderStatus Execute(Random Random)
		{
			if (Validate() == OrderInvalidReason.NONE)
			{
				Unit.Dismount(UseMovement);
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}
	}
}
