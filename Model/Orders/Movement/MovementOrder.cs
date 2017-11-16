using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

using Cardamom.Graphing;

namespace PanzerBlitz
{
	public class MovementOrder : Order
	{
		public readonly Unit Unit;
		public readonly bool Combat;
		public readonly bool Halt;
		public readonly Path<Tile> Path;

		public Army Army
		{
			get
			{
				return Unit.Army;
			}
		}

		public MovementOrder(Unit Unit, Tile To, bool Combat, bool Halt = false)
		{
			this.Unit = Unit;
			this.Combat = Combat;
			this.Halt = Halt;
			this.Path = Unit.GetPathTo(To, Combat);
		}

		public MovementOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this(
				(Unit)Objects[Stream.ReadInt32()],
				(Tile)Objects[Stream.ReadInt32()],
				Stream.ReadBoolean(),
				Stream.ReadBoolean())
		{ }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Unit.Id);
			Stream.Write(Path.Destination.Id);
			Stream.Write(Combat);
			Stream.Write(Halt);
		}

		public bool MatchesTurnComponent(TurnComponent TurnComponent)
		{
			if (Combat)
			{
				return Unit.Configuration.IsVehicle
						   ? TurnComponent == TurnComponent.VEHICLE_COMBAT_MOVEMENT
							   : TurnComponent == TurnComponent.CLOSE_ASSAULT;
			}
			return Unit.Configuration.IsVehicle
					   ? TurnComponent == TurnComponent.VEHICLE_MOVEMENT
						   : TurnComponent == TurnComponent.NON_VEHICLE_MOVEMENT;
		}

		public OrderInvalidReason Validate()
		{
			if (Unit.CanMove(Combat) != OrderInvalidReason.NONE) return OrderInvalidReason.UNIT_NO_MOVE;

			OrderInvalidReason noEnter = Unit.CanEnter(Path.Destination, true);
			if (noEnter != OrderInvalidReason.NONE) return noEnter;

			for (int i = 0; i < Path.Count - 1; ++i)
			{
				noEnter = Unit.CanEnter(Path[i + 1]);
				if (noEnter != OrderInvalidReason.NONE) return noEnter;

				float d = Path[i].RulesCalculator.GetMoveCost(Unit, Path[i + 1], !Combat);
				if (Math.Abs(d - float.MaxValue) < float.Epsilon) return OrderInvalidReason.MOVEMENT_TERRAIN;
			}
			if (Path.Distance > Unit.RemainingMovement)
			{
				if (!Unit.Moved && Path.Count <= 2) return OrderInvalidReason.NONE;
				return OrderInvalidReason.UNIT_NO_MOVE;
			}
			if (Combat && Unit.Configuration.CanCloseAssault && Path.Count > 2) return OrderInvalidReason.UNIT_NO_MOVE;
			return OrderInvalidReason.NONE;
		}

		public OrderStatus Execute(Random Random)
		{
			if (Validate() == OrderInvalidReason.NONE)
			{
				Unit.MoveTo(Path.Destination, Path);
				if (Halt) Unit.Halt();

				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}
	}
}
