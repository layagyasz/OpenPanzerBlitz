﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class OneOfZoneDeployment : PositionalDeployment
	{
		public readonly OneOfZoneDeploymentConfiguration DeploymentConfiguration;

		public override DeploymentConfiguration Configuration
		{
			get
			{
				return DeploymentConfiguration;
			}
		}

		public OneOfZoneDeployment(
			Army Army,
			IEnumerable<Unit> Units,
			OneOfZoneDeploymentConfiguration DeploymentConfiguration,
			IdGenerator IdGenerator)
			: base(Army, Units, IdGenerator)
		{
			this.DeploymentConfiguration = DeploymentConfiguration;
		}

		public override bool UnitMustMove(Unit Unit)
		{
			return false;
		}

		public override NoDeployReason Validate(Unit Unit, Tile Tile)
		{
			NoDeployReason v = base.Validate(Unit, Tile);
			if (v != NoDeployReason.NONE) return v;

			if (Tile != null)
			{
				Matcher<Tile> m = DeploymentConfiguration.Matchers.FirstOrDefault(
					i => Units.Any(j => j.Position != null && i.Matches(j.Position)));

				if (m != null && !m.Matches(Tile)) return NoDeployReason.DEPLOYMENT_RULE;
				if (m == null && !DeploymentConfiguration.Matchers.Any(i => i.Matches(Tile)))
					return NoDeployReason.DEPLOYMENT_RULE;
			}
			return NoDeployReason.NONE;
		}
	}
}
