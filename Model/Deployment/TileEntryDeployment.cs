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

		public override bool UnitMustMove(Unit Unit)
		{
			if (Unit.Position == null) return false;
			if (!_StopAutomatedMovement) return true;
			return base.UnitMustMove(Unit);
		}

		public override void ReassessMatch()
		{
			if (!_StopAutomatedMovement
				&& DeploymentConfiguration.MovementAutomator != null
				&& DeploymentConfiguration.MovementAutomator.StopEarly(Army))
			{
				_StopAutomatedMovement = true;
			}
		}

		public override void EnterUnits(bool Vehicle)
		{
			Unit unit = _ConvoyOrder.FirstOrDefault(i => i.Position == null);
			if (unit != null && unit.Configuration.IsVehicle == Vehicle)
			{
				Army.Match.ExecuteOrder(new MovementDeployOrder(unit, _EntryTile));
			}
		}

		public override void AutomateMovement(bool Vehicle)
		{
			if (DeploymentConfiguration.MovementAutomator != null && !_StopAutomatedMovement)
			{
				foreach (Unit u in _ConvoyOrder)
				{
					if (u.Position != null
						&& u.CanMove(Vehicle, false) == NoMoveReason.NONE
						&& !u.Moved
						&& DeploymentConfiguration.MovementAutomator.AutomateMovement(u, _StopAutomatedMovement))
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
