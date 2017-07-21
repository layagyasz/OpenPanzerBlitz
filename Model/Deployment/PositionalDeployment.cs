using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public abstract class PositionalDeployment : Deployment
	{
		public PositionalDeployment(Army Army, IEnumerable<Unit> Units)
			: base(Army, Units)
		{
		}

		public virtual NoDeployReason Validate(Unit Unit, Tile Tile)
		{
			if (Tile == null) return NoDeployReason.NONE;
			if (!Tile.Units.Contains(Unit)
				&& Tile.GetStackSize() >= Unit.Army.Configuration.Faction.StackLimit)
				return NoDeployReason.STACK_LIMIT;
			return NoDeployReason.NONE;
		}
	}
}
