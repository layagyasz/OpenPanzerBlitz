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

		public NoMoveReason Validate()
		{
			if (!Unit.CanExitDirection(Direction)) return NoMoveReason.ILLEGAL;
			if (Unit.CanMove(false) != NoMoveReason.NONE) return Unit.CanMove(false);
			return NoMoveReason.NONE;
		}

		public OrderStatus Execute(Random Random)
		{
			if (Validate() == NoMoveReason.NONE)
			{
				Unit.Remove();
				Unit.Evacuated = Direction;
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}
	}
}
