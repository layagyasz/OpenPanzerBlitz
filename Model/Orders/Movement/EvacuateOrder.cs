using System;
using System.Collections.Generic;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class EvacuateOrder : Order
	{
		public readonly Unit Unit;
		public readonly Direction Direction;

		public Army Army
		{
			get
			{
				return Unit.Army;
			}
		}

		public EvacuateOrder(Unit Unit, Direction Direction)
		{
			this.Unit = Unit;
			this.Direction = Direction;
		}

		public EvacuateOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Unit)Objects[Stream.ReadInt32()], (Direction)Stream.ReadByte()) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Unit.Id);
			Stream.Write((byte)Direction);
		}

		public bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return Unit.Configuration.IsVehicle
					   ? TurnComponent == TurnComponent.VEHICLE_MOVEMENT
							: TurnComponent == TurnComponent.NON_VEHICLE_MOVEMENT;
		}

		public OrderInvalidReason Validate()
		{
			if (!Unit.CanExitDirection(Direction)) return OrderInvalidReason.ILLEGAL;
			if (Unit.CanMove(false) != OrderInvalidReason.NONE) return Unit.CanMove(false);
			return OrderInvalidReason.NONE;
		}

		public OrderStatus Execute(Random Random)
		{
			if (Validate() == OrderInvalidReason.NONE)
			{
				Unit.Evacuated = Unit.Position;
				Unit.Remove();
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}
	}
}
