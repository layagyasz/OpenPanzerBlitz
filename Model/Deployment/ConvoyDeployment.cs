using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class ConvoyDeployment : Deployment
	{
		public readonly ConvoyDeploymentConfiguration DeploymentConfiguration;

		protected Tile _EntryTile;
		protected List<Unit> _ConvoyOrder;
		bool _StopAutomatedMovement;

		public override DeploymentConfiguration Configuration
		{
			get
			{
				return DeploymentConfiguration;
			}
		}
		public Tile EntryTile
		{
			get
			{
				return _EntryTile;
			}
		}
		public IEnumerable<Unit> ConvoyOrder
		{
			get
			{

				return _ConvoyOrder;
			}
		}
		public bool IsStrictConvoy
		{
			get
			{
				return DeploymentConfiguration.IsStrictConvoy;
			}
		}

		public ConvoyDeployment(
				Army Army,
				IEnumerable<Unit> Units,
				ConvoyDeploymentConfiguration DeploymentConfiguration,
				IdGenerator IdGenerator)
			: base(Army, Units, IdGenerator)
		{
			this.DeploymentConfiguration = DeploymentConfiguration;
			_StopAutomatedMovement = DeploymentConfiguration.MovementAutomator == null;
		}

		public override bool AutomateDeployment()
		{
			var validTiles = Army.Match.Map.TilesEnumerable.Where(
				i => Validate(i) == OrderInvalidReason.NONE).ToList();
			if (validTiles.Count == 1) Army.Match.ExecuteOrder(new EntryTileDeployOrder(this, validTiles.First()));
			if (validTiles.Count == 0) throw new Exception("No valid entry tiles for ConvoyDeployment.");
			return false;
		}

		public override bool UnitMustMove(Unit Unit)
		{
			if (Unit.Position == null) return false;
			if (!_StopAutomatedMovement) return true;
			return Unit.Position == _EntryTile && _ConvoyOrder.Any(i => !i.Deployed);
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
			var unit = _ConvoyOrder.FirstOrDefault(i => i.Position == null && i.Status == UnitStatus.ACTIVE);
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
						&& u.CanMove(Vehicle, false) == OrderInvalidReason.NONE
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
			return Validate(_EntryTile) == OrderInvalidReason.NONE && Validate(_ConvoyOrder) == OrderInvalidReason.NONE;
		}

		public override OrderInvalidReason Validate(Unit Unit, Tile Tile)
		{
			var r = base.Validate(Unit, Tile);
			if (r != OrderInvalidReason.NONE) return r;

			if (_EntryTile.Units.Count() > 0) return OrderInvalidReason.DEPLOYMENT_RULE;
			if (Tile != _EntryTile) return OrderInvalidReason.DEPLOYMENT_RULE;
			return OrderInvalidReason.NONE;
		}

		public virtual OrderInvalidReason Validate(Tile EntryTile)
		{
			if (!DeploymentConfiguration.Matcher.Matches(EntryTile))
				return OrderInvalidReason.DEPLOYMENT_RULE;

			return OrderInvalidReason.NONE;
		}

		public OrderInvalidReason Validate(IEnumerable<Unit> ConvoyOrder)
		{
			if (ConvoyOrder == null) return OrderInvalidReason.DEPLOYMENT_CONVOY_ORDER;
			if (!Units.All(i => ConvoyOrder.Any(j => i == j || j.Passenger == i)))
				return OrderInvalidReason.DEPLOYMENT_CONVOY_ORDER;
			if (Units.Where(i => i.Configuration.IsPassenger)
					.Any(i => (i.Configuration.Movement == 0 || IsStrictConvoy) && i.Carrier == null))
				return OrderInvalidReason.DEPLOYMENT_CONVOY_ORDER;
			return OrderInvalidReason.NONE;
		}

		public void SetEntryTile(Tile Tile)
		{
			if (Validate(Tile) == OrderInvalidReason.NONE) _EntryTile = Tile;
		}

		public void SetConvoyOrder(IEnumerable<Unit> ConvoyOrder)
		{
			if (Validate(ConvoyOrder) == OrderInvalidReason.NONE) _ConvoyOrder = ConvoyOrder.ToList();
		}
	}
}
