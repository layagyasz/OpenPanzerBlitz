using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class NextPhaseOrder : Order
	{
		public Army Army { get; }
		public TurnComponent TurnComponent { get; }

		public NextPhaseOrder(TurnInfo Turn)
			: this(Turn.Army, Turn.TurnComponent) { }

		public NextPhaseOrder(Army Army, TurnComponent TurnComponent)
		{
			this.Army = Army;
			this.TurnComponent = TurnComponent;
		}

		public NextPhaseOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Army)Objects[Stream.ReadInt32()], (TurnComponent)Stream.ReadByte()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Army.Id);
			Stream.Write((byte)TurnComponent);
		}

		public bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return this.TurnComponent == TurnComponent;
		}

		public Order CloneIfStateful()
		{
			return this;
		}

		public OrderInvalidReason Validate()
		{
			if (TurnComponent == TurnComponent.DEPLOYMENT && !Army.IsDeploymentConfigured())
				return OrderInvalidReason.DEPLOYMENT_RULE;
			if (TurnComponent == TurnComponent.NON_VEHICLE_MOVEMENT && Army.MustMove(false))
				return OrderInvalidReason.UNIT_MUST_MOVE;
			if (TurnComponent == TurnComponent.VEHICLE_MOVEMENT && Army.MustMove(true))
				return OrderInvalidReason.UNIT_MUST_MOVE;
			return OrderInvalidReason.NONE;
		}

		public OrderStatus Execute(Random Random)
		{
			Army.Reset(false);
			return OrderStatus.FINISHED;
		}

		public override string ToString()
		{
			return string.Format("[NextPhaseOrder: Army={0}, TurnComponent={1}]", Army, TurnComponent);
		}
	}
}
