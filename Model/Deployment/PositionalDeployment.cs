using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public abstract class PositionalDeployment : Deployment
	{
		public PositionalDeployment(IEnumerable<Unit> Units)
			: base(Units)
		{
		}

		public virtual NoDeployReason Validate(Unit Unit, Tile Tile)
		{
			if (Tile == null) return NoDeployReason.NONE;
			if (!Tile.Units.Contains(Unit)
				&& Tile.GetStackSize() >= Unit.Army.ArmyConfiguration.Faction.StackLimit)
				return NoDeployReason.STACK_LIMIT;
			return NoDeployReason.NONE;
		}
	}
}
