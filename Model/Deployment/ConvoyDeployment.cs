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
		}

		public override bool AutomateDeployment()
		{
			List<Tile> validTiles = Army.Match.Map.TilesEnumerable.Where(
				i => Validate(i) == NoDeployReason.NONE).ToList();
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

		public override NoDeployReason Validate(Unit Unit, Tile Tile)
		{
			NoDeployReason r = base.Validate(Unit, Tile);
			if (r != NoDeployReason.NONE) return r;

			if (_EntryTile.Units.Count() > 0) return NoDeployReason.DEPLOYMENT_RULE;
			if (Tile != _EntryTile) return NoDeployReason.DEPLOYMENT_RULE;
			return NoDeployReason.NONE;
		}

		public virtual NoDeployReason Validate(Tile EntryTile)
		{
			if (!DeploymentConfiguration.Matcher.Matches(EntryTile))
				return NoDeployReason.DEPLOYMENT_RULE;

			return NoDeployReason.NONE;
		}

		public NoDeployReason Validate(IEnumerable<Unit> ConvoyOrder)
		{
			if (_ConvoyOrder == null) return NoDeployReason.CONVOY_ORDER;
			if (!Units.All(i => ConvoyOrder.Any(j => i == j || j.Passenger == i))) return NoDeployReason.CONVOY_ORDER;
			if (Units.Where(i => i.Configuration.IsPassenger)
					.Any(i => (i.Configuration.Movement == 0 || IsStrictConvoy) && i.Carrier == null))
				return NoDeployReason.CONVOY_ORDER;
			return NoDeployReason.NONE;
		}

		public void SetEntryTile(Tile Tile)
		{
			if (Validate(Tile) == NoDeployReason.NONE) _EntryTile = Tile;
		}

		public void SetConvoyOrder(IEnumerable<Unit> ConvoyOrder)
		{
			if (Validate(ConvoyOrder) == NoDeployReason.NONE) _ConvoyOrder = ConvoyOrder.ToList();
		}
	}
}
