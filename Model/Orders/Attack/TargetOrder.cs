using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TargetOrder : Order
	{
		public readonly Unit Unit;
		public readonly Tile Tile;

		public Army Army
		{
			get
			{
				return Unit.Army;
			}
		}

		public TargetOrder(Unit Unit, Tile Tile)
		{
			this.Unit = Unit;
			this.Tile = Tile;
		}

		public TargetOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Unit)Objects[Stream.ReadInt32()], (Tile)Objects[Stream.ReadInt32()]) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Unit.Id);
			Stream.Write(Tile.Id);
		}

		public bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return TurnComponent == TurnComponent.ARTILLERY;
		}

		public Order CloneIfStateful()
		{
			return this;
		}

		public OrderInvalidReason Validate()
		{
			return Unit.CanAttack(AttackMethod.INDIRECT_FIRE, false, new LineOfSight(Unit.Position, Tile), false);
		}

		public OrderStatus Execute(Random Random)
		{
			if (Validate() == OrderInvalidReason.NONE)
			{
				Unit.Target = Tile;
				Unit.Halt();
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}

		public override string ToString()
		{
			return string.Format("[TargetOrder: Unit={0}, Tile={1}]", Unit, Tile);
		}
	}
}
