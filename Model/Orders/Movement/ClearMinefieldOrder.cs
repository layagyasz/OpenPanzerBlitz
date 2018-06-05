using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ClearMinefieldOrder : Order
	{
		public readonly Unit Engineer;
		public readonly Unit Minefield;

		ClearMinefieldInteraction _Interaction;
		bool _PreExisting;

		public Army Army
		{
			get
			{
				return Engineer.Army;
			}
		}

		public ClearMinefieldOrder(Unit Engineer, Unit Minefield)
		{
			this.Engineer = Engineer;
			this.Minefield = Minefield;
		}

		public ClearMinefieldOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Unit)Objects[Stream.ReadInt32()], (Unit)Objects[Stream.ReadInt32()]) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Engineer.Id);
			Stream.Write(Minefield.Id);
		}

		public bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			return Engineer.Configuration.IsVehicle
						   ? TurnComponent == TurnComponent.VEHICLE_MOVEMENT
							   : TurnComponent == TurnComponent.NON_VEHICLE_MOVEMENT;
		}

		public Order CloneIfStateful()
		{
			return this;
		}

		public OrderInvalidReason Validate()
		{
			var r = Engineer.CanClearMinefield();
			if (r != OrderInvalidReason.NONE) return r;

			_Interaction =
				Engineer.HasInteraction<ClearMinefieldInteraction>(i => i.Agent == Engineer && i.Object == Minefield);
			_PreExisting = _Interaction != null;
			if (_PreExisting)
			{
				if (Engineer.Position == null || Engineer.Position != Minefield.Position)
					return OrderInvalidReason.TARGET_OUT_OF_RANGE;
				return _Interaction.Validate();
			}

			if (Minefield.Interactions.Count() > 0)
				return OrderInvalidReason.TARGET_ALREADY_ATTACKED;
			_Interaction = new ClearMinefieldInteraction(Engineer, Minefield);
			return _Interaction.Validate();
		}

		public OrderStatus Execute(Random Random)
		{
			if (Validate() == OrderInvalidReason.NONE)
			{
				if (_PreExisting)
				{
					Minefield.HandleCombatResult(CombatResult.DESTROY, AttackMethod.NONE, null);
					_Interaction.Cancel();
				}
				else
				{
					_Interaction.Apply(Minefield);
					Engineer.CancelInteractions();
					Engineer.AddInteraction(_Interaction);
					Minefield.AddInteraction(_Interaction);
				}
				Engineer.Halt();
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}
	}
}
