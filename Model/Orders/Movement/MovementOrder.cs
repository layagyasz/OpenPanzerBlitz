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
		{
			Unit = (Unit)Objects[Stream.ReadInt32()];
			Tile from = (Tile)Objects[Stream.ReadInt32()];
			Tile to = (Tile)Objects[Stream.ReadInt32()];
			Combat = Stream.ReadBoolean();
			Halt = Stream.ReadBoolean();
			Path = Unit.GetPathTo(from, to, Combat);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Unit.Id);
			Stream.Write(Path.Nodes.First().Id);
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
			if (Unit.Position != Path.Nodes.First()) return OrderInvalidReason.ILLEGAL;

			OrderInvalidReason noEnter = Unit.CanEnter(Path.Destination, true);
			if (noEnter != OrderInvalidReason.NONE) return noEnter;

			for (int i = 0; i < Path.Count - 1; ++i)
			{
				noEnter = Unit.CanEnter(Path[i + 1]);
				if (noEnter != OrderInvalidReason.NONE) return noEnter;

				MovementCost d = Path[i].RulesCalculator.GetMoveCost(Unit, Path[i + 1], !Combat);
				if (d.UnableReason != OrderInvalidReason.NONE) return d.UnableReason;
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
				if (Unit.Configuration.InnatelyClearsMines)
				{
					foreach (Unit minefield in Path.Nodes
							 .Take(Path.Count - 1)
							 .SelectMany(i => i.Units).Where(j => j.Configuration.UnitClass == UnitClass.MINEFIELD)
							 .ToList())
						minefield.HandleCombatResult(CombatResult.DESTROY);

				}
				return OrderStatus.FINISHED;
			}
			return OrderStatus.ILLEGAL;
		}

		public override string ToString()
		{
			return string.Format(
				"[MovementOrder: Unit={0}, From={1}, To={2}, Distance={3}]",
				Unit,
				Path.Nodes.First(),
				Path.Destination,
				Path.Distance);
		}
	}
}
