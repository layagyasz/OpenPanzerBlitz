using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class FortifyOrder : Order
	{
		public readonly Unit Unit;

		Unit _Fort;

		public Army Army
		{
			get
			{
				return Unit.Army;
			}
		}

		public FortifyOrder(Unit Unit)
		{
			this.Unit = Unit;
		}

		public FortifyOrder(SerializationInputStream Stream, List<GameObject> Objects)
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
			if (Unit.CanFortify() != OrderInvalidReason.NONE) return Unit.CanFortify();
			foreach (Unit unit in Unit.Position.Units)
			{
				var r = Validate(unit);
				if (r == OrderInvalidReason.NONE)
				{
					_Fort = unit;
					return r;
				}
			}
			return OrderInvalidReason.UNIT_NO_FORT;
		}

		OrderInvalidReason Validate(Unit Fort)
		{
			if (Fort == null) return OrderInvalidReason.UNIT_NO_FORT;
			if (Fort.Configuration.UnitClass != UnitClass.FORT) return OrderInvalidReason.UNIT_NO_FORT;
			if (Unit.Position == null
				|| Fort.Position == null
				|| Unit.Position != Fort.Position)
				return OrderInvalidReason.TARGET_OUT_OF_RANGE;
			if (Unit.Army != Fort.Army) return OrderInvalidReason.TARGET_TEAM;
			if (Fort.Interactions.Where(i => i is FortifyInteraction)
				.Sum(i => i.Master.Configuration.GetStackSize()) >= Fort.Army.Configuration.Faction.StackLimit)
				return OrderInvalidReason.UNIT_STACK_LIMIT;
			return OrderInvalidReason.NONE;
		}

		public OrderStatus Execute(Random Random)
		{
			if (Validate() == OrderInvalidReason.NONE)
			{
				Unit.Fortify(_Fort);
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}
	}
}
