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

		public override bool IsStrictConvoy
		{
			get
			{
				return DeploymentConfiguration.IsStrictConvoy;
			}
		}

		public TileEntryDeployment(
			Army Army,
			IEnumerable<Unit> Units,
			TileEntryDeploymentConfiguration DeploymentConfiguration,
			IdGenerator IdGenerator)
			: base(Army, Units, IdGenerator)
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

		public override NoDeployReason Validate(Tile Tile)
		{
			NoDeployReason v = base.Validate(Tile);
			if (v != NoDeployReason.NONE) return v;

			if (!DeploymentConfiguration.Matcher.Matches(Tile))
				return NoDeployReason.DEPLOYMENT_RULE;

			return NoDeployReason.NONE;
		}
	}
}
