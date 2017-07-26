using System;
using System.Linq;

using Cardamom.Graphing;

namespace PanzerBlitz
{
	public class MovementOrder : Order
	{
		public readonly Unit Unit;
		public readonly bool Combat;
		public readonly Path<Tile> Path;

		public MovementOrder(Unit Unit, Tile To, bool Combat)
		{
			this.Unit = Unit;
			this.Combat = Combat;
			this.Path = Unit.GetPathTo(To, Combat);
		}

		public NoMoveReason Validate()
		{
			if (Unit.CanMove(Combat) != NoMoveReason.NONE) return NoMoveReason.NO_MOVE;

			NoDeployReason noEnter = Unit.CanEnter(Path.Destination, true);
			if (noEnter != NoDeployReason.NONE) return EnumConverter.ConvertToNoMoveReason(noEnter);

			for (int i = 0; i < Path.Count - 1; ++i)
			{
				noEnter = Unit.CanEnter(Path[i + 1]);
				if (noEnter != NoDeployReason.NONE) return EnumConverter.ConvertToNoMoveReason(noEnter);

				float d = Path[i].Configuration.GetMoveCost(Unit, Path[i + 1], !Combat);
				if (Math.Abs(d - float.MaxValue) < float.Epsilon) return NoMoveReason.TERRAIN;
			}
			if (Path.Distance > Unit.RemainingMovement)
			{
				if (!Unit.Moved && Path.Count <= 2) return NoMoveReason.NONE;
				return NoMoveReason.NO_MOVE;
			}
			if (Combat && Unit.Configuration.CanCloseAssault && Path.Count > 2) return NoMoveReason.NO_MOVE;
			return NoMoveReason.NONE;
		}

		public bool Execute(Random Random)
		{
			if (Validate() == NoMoveReason.NONE)
			{
				Unit.MoveTo(Path.Destination, (float)Path.Distance);
				return true;
			}
			return false;
		}
	}
}
