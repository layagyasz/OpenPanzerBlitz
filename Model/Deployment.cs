using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public abstract class Deployment
	{
		public readonly List<Unit> Units;

		protected bool _DeploymentComplete;

		public bool DeploymentComplete
		{
			get
			{
				return _DeploymentComplete;
			}
		}

		public Deployment(IEnumerable<Unit> Units, DeploymentConfiguration DeploymentConfiguration)
		{
			this.Units = Units.ToList();
		}

		public abstract bool AutomateDeployment(Match Match);
		public abstract bool IsConfigured();

		public virtual NoDeployReason Validate(Unit Unit, Tile Tile)
		{
			if (Tile == null) return NoDeployReason.NONE;
			if (!Tile.Units.Contains(Unit)
				&& Tile.GetStackSize() >= Unit.Army.ArmyConfiguration.Faction.StackLimit)
				return NoDeployReason.STACK_LIMIT;
			return NoDeployReason.NONE;
		}

		public bool Validate()
		{
			return Units.Where(i => !i.Deployed).All(i => Validate(i, i.Position) == NoDeployReason.NONE);
		}
	}
}
