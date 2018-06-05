using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class NextPhaseOrder : Order
	{
		public Army Army { get; }

		public NextPhaseOrder(Army Army)
		{
			this.Army = Army;
		}

		public NextPhaseOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Army)Objects[Stream.ReadInt32()]) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Army.Id);
		}

		public bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			switch (TurnComponent)
			{
				case TurnComponent.DEPLOYMENT: return Army.IsDeploymentConfigured();
				case TurnComponent.NON_VEHICLE_MOVEMENT: return !Army.MustMove(false);
				case TurnComponent.VEHICLE_MOVEMENT: return !Army.MustMove(true);
				default: return true;
			}
		}

		public Order CloneIfStateful()
		{
			return this;
		}

		public OrderInvalidReason Validate()
		{
			return OrderInvalidReason.NONE;
		}

		public OrderStatus Execute(Random Random)
		{
			Army.Reset(false);
			return OrderStatus.FINISHED;
		}
	}
}
