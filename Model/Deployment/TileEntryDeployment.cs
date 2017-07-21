using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class TileEntryDeployment : ConvoyDeployment
	{
		public readonly TileEntryDeploymentConfiguration DeploymentConfiguration;

		public override DeploymentConfiguration Configuration
		{
			get
			{
				return DeploymentConfiguration;

			}
		}

		public TileEntryDeployment(
			Army Army, IEnumerable<Unit> Units, TileEntryDeploymentConfiguration DeploymentConfiguration)
			: base(Army, Units)
		{
			this.DeploymentConfiguration = DeploymentConfiguration;
		}

		public override bool AutomateDeployment(Match Match)
		{
			return false;
		}

		public override bool IsConfigured()
		{
			return Validate(_EntryTile) == NoDeployReason.NONE && Validate(_ConvoyOrder) == NoDeployReason.NONE;
		}
	}
}
