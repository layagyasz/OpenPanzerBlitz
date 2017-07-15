using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Window;

namespace PanzerBlitz
{
	public class ZoneDeployment : Deployment
	{
		public readonly ZoneDeploymentConfiguration DeploymentConfiguration;

		public ZoneDeployment(IEnumerable<Unit> Units, ZoneDeploymentConfiguration DeploymentConfiguration)
			: base(Units, DeploymentConfiguration)
		{
			this.DeploymentConfiguration = DeploymentConfiguration;
		}

		public override bool AutomateDeployment(Match Match)
		{
			return false;
		}

		public override bool IsConfigured()
		{
			return Units.All(i => i.Position != null && Validate(i, i.Position) == NoDeployReason.NONE);
		}

		public override NoDeployReason Validate(Unit Unit, Tile Tile)
		{
			NoDeployReason v = base.Validate(Unit, Tile);
			if (v != NoDeployReason.NONE) return v;

			if (Tile != null)
			{
				if (!DeploymentConfiguration.Zone.ContainsPoint(new Vector2f(Tile.X, Tile.Y)))
					return NoDeployReason.DEPLOYMENT_RULE;
			}
			return NoDeployReason.NONE;
		}
	}
}
