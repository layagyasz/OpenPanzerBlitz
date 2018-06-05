using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class EmplaceOrder : Order
	{
		public readonly Unit Engineer;
		public readonly Unit Target;

		EmplaceInteraction _Interaction;
		bool _PreExisting;

		public Army Army
		{
			get
			{
				return Engineer.Army;
			}
		}

		public EmplaceOrder(Unit Engineer, Unit Target)
		{
			this.Engineer = Engineer;
			this.Target = Target;
		}
		public EmplaceOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Unit)Objects[Stream.ReadInt32()], (Unit)Objects[Stream.ReadInt32()]) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Engineer.Id);
			Stream.Write(Target.Id);
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
			var r = Target.Configuration.UnitClass == UnitClass.MINEFIELD
						  ? Engineer.CanPlaceMinefield()
						  : Engineer.CanPlaceBridge();
			if (r != OrderInvalidReason.NONE) return r;
			r = Target.CanBeEmplaced();
			if (r != OrderInvalidReason.NONE) return r;

			var requirements = EmplacementRequirements.GetEmplacementRequirementsFor(Target);
			_Interaction =
				Engineer.HasInteraction<EmplaceInteraction>(i => i.Agent == Engineer && i.Object == Target);
			_PreExisting = _Interaction != null;
			if (_PreExisting) return _Interaction.Validate();

			if (Target.Interactions.Count(i => i is EmplaceOrder) >= requirements.MaxUnits)
				return OrderInvalidReason.TARGET_INTERACTION_LIMIT;
			_Interaction = new EmplaceInteraction(Engineer, Target);
			return _Interaction.Validate();
		}

		public OrderStatus Execute(Random Random)
		{
			if (Validate() == OrderInvalidReason.NONE)
			{
				_Interaction.Tick();
				if (!_PreExisting)
				{
					Engineer.CancelInteractions();
					Engineer.AddInteraction(_Interaction);
					Target.AddInteraction(_Interaction);
				}
				if (Finished())
				{
					_Interaction.Cancel();
					Target.Emplace(true);
				}
				Engineer.Halt();
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}

		bool Finished()
		{
			var requirements = EmplacementRequirements.GetEmplacementRequirementsFor(Target);
			var interactions = Target.Interactions.Where(i => i is EmplaceInteraction).Cast<EmplaceInteraction>();
			var count = interactions.Count();
			var turns = requirements.PoolTurns ? interactions.Sum(i => i.Turns) : interactions.Min(i => i.Turns);

			return count >= requirements.MinUnits && count <= requirements.MaxUnits && turns >= requirements.Turns;
		}
	}
}
