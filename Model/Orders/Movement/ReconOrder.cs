﻿using System;
using System.Collections.Generic;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ReconOrder : Order
	{
		public readonly Unit Unit;
		public readonly Direction Direction;
		public readonly int Turns;
		public readonly Tile ExitTile;

		int _RemainingTurns;

		public Army Army
		{
			get
			{
				return Unit.Army;
			}
		}

		public ReconOrder(Unit Unit, Direction Direction)
		{
			this.Unit = Unit;
			this.Direction = Direction;

			Turns = 3;
			ExitTile = Unit.Position;
			_RemainingTurns = Turns;
		}

		public ReconOrder(SerializationInputStream Stream, List<GameObject> Objects)
			: this((Unit)Objects[Stream.ReadInt32()], (Direction)Stream.ReadByte())
		{
			Turns = Stream.ReadInt32();
			ExitTile = (Tile)Objects[Stream.ReadInt32()];
			_RemainingTurns = (int)Stream.ReadInt32();
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Unit.Id);
			Stream.Write((byte)Direction);
			Stream.Write(Turns);
			Stream.Write(ExitTile.Id);
			Stream.Write(_RemainingTurns);
		}

		public NoMoveReason Validate()
		{
			if (!Unit.CanExitDirection(Direction)) return NoMoveReason.ILLEGAL;
			if (Unit.CanMove(false) != NoMoveReason.NONE) return Unit.CanMove(false);
			return NoMoveReason.NONE;
		}

		public OrderStatus Execute(Random Random)
		{
			if (_RemainingTurns == Turns)
			{
				if (Validate() != NoMoveReason.NONE) return OrderStatus.ILLEGAL;
				Unit.Remove();
			}
			if (_RemainingTurns-- <= 0)
			{
				if (Unit.CanEnter(ExitTile, true) != NoDeployReason.NONE)
				{
					Unit.Recon(Direction);
					Unit.Place(ExitTile);
					return OrderStatus.FINISHED;
				}
			}
			return OrderStatus.IN_PROGRESS;
		}
	}
}
