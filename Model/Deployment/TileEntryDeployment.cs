using System;
using System.Collections.Generic;
using System.Linq;

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

		bool _StopAutomatedMovement;

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
			Unit unit = _ConvoyOrder.FirstOrDefault(i => i.Position == null);
			if (unit != null && unit.Configuration.IsVehicle == Vehicle)
			{
				Match.ExecuteOrder(new MovementDeployOrder(unit, _EntryTile));
			}

			if (DeploymentConfiguration.MovementAutomator != null && !_StopAutomatedMovement)
			{
				foreach (Unit u in _ConvoyOrder)
				{
					if (u.Position != null && DeploymentConfiguration.MovementAutomator.AutomateMovement(u, Match))
					{
						_StopAutomatedMovement = true;
					}
				}
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
