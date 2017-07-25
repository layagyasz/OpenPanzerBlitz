using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Window;

namespace PanzerBlitz
{
	public class ZoneDeployment : PositionalDeployment
	{
		public readonly ZoneDeploymentConfiguration DeploymentConfiguration;

		public override DeploymentConfiguration Configuration
		{
			get
			{
				return DeploymentConfiguration;
			}
		}

		public ZoneDeployment(Army Army, IEnumerable<Unit> Units, ZoneDeploymentConfiguration DeploymentConfiguration)
			: base(Army, Units)
		{
			this.DeploymentConfiguration = DeploymentConfiguration;
		}

		public override bool AutomateDeployment(Match Match)
		{
			return false;
		}

		public override void AutomateMovement(Match Match, bool Vehicle)
		{
		}

		public override bool IsConfigured()
		{
			return Units.All(i => i.Position != null);
		}

		public override NoDeployReason Validate(Unit Unit, Tile Tile)
		{
			NoDeployReason v = base.Validate(Unit, Tile);
			if (v != NoDeployReason.NONE) return v;

			if (Tile != null)
			{
				if (!DeploymentConfiguration.Matcher.Matches(Tile))
					return NoDeployReason.DEPLOYMENT_RULE;
			}
			return NoDeployReason.NONE;
		}
	}
}
