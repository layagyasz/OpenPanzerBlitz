using System;
using System.Collections.Generic;

using SFML.Window;

namespace PanzerBlitz
{
	public class ZoneDeployment : Deployment
	{
		public readonly ZoneDeploymentConfiguration DeploymentConfiguration;

		public ZoneDeployment(Army Army, ZoneDeploymentConfiguration DeploymentConfiguration)
			: base(Army, DeploymentConfiguration)
		{
			this.DeploymentConfiguration = DeploymentConfiguration;
		}

		public override void AutomateDeployment(Match Match) { }

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
