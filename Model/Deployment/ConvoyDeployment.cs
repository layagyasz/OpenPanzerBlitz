using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public abstract class ConvoyDeployment : Deployment
	{
		protected Tile _EntryTile;
		protected List<Unit> _ConvoyOrder;

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

		public abstract bool IsStrictConvoy { get; }

		public ConvoyDeployment(Army Army, IEnumerable<Unit> Units, IdGenerator IdGenerator)
			: base(Army, Units, IdGenerator)
		{
		}

		public override bool AutomateDeployment(Match Match)
		{
			return false;
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
			if (EntryTile != null && EntryTile.NeighborTiles.Any(i => i == null)) return NoDeployReason.NONE;
			return NoDeployReason.DEPLOYMENT_RULE;
		}

		public NoDeployReason Validate(IEnumerable<Unit> ConvoyOrder)
		{
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
