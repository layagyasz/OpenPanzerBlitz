using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public abstract class Deployment
	{
		public readonly Army Army;
		public readonly List<Unit> Units;
		public abstract DeploymentConfiguration Configuration { get; }

		public Deployment(Army Army, IEnumerable<Unit> Units)
		{
			this.Army = Army;
			this.Units = Units.ToList();
		}

		public virtual NoDeployReason Validate(Unit Unit, Tile Tile)
		{
			if (Tile == null) return NoDeployReason.NONE;
			return Unit.CanEnter(Tile, true);
		}

		public abstract bool AutomateDeployment(Match Match);
		public abstract void AutomateMovement(Match Match, bool Vehicle);
		public abstract bool IsConfigured();
	}
}
