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

		public override void AutomateMovement(Match Match, bool Vehicle)
		{
			if (_ConvoyOrder.Count > 0 && _ConvoyOrder[0].Configuration.IsVehicle == Vehicle)
			{
				DeployOrder order = new MovementDeployOrder(_ConvoyOrder[0], _EntryTile);
				if (Match.ExecuteOrder(order)) _ConvoyOrder.RemoveAt(0);
			}
		}

		public override bool IsConfigured()
		{
			return Validate(_EntryTile) == NoDeployReason.NONE && Validate(_ConvoyOrder) == NoDeployReason.NONE;
		}
	}
}
