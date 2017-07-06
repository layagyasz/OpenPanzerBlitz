using System;

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
			for (int i = 0; i < Path.Count - 1; ++i)
			{
				double d = Path[i].MovementProfile.GetMoveCost(Unit, Path[i + 1], !Combat);
				if (Math.Abs(d - double.MaxValue) < double.Epsilon) return NoMoveReason.TERRAIN;
			}
			if (Path.Distance > Unit.RemainingMovement) return NoMoveReason.NO_MOVE;
			if (Combat && Unit.UnitConfiguration.CanCloseAssault && Path.Distance > 1) return NoMoveReason.NO_MOVE;
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
