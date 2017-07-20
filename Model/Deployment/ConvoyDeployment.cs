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

		public ConvoyDeployment(IEnumerable<Unit> Units)
			: base(Units)
		{
		}

		public NoDeployReason Validate(Tile EntryTile)
		{
			if (_EntryTile != null && _EntryTile.NeighborTiles.Any(i => i == null)) return NoDeployReason.NONE;
			return NoDeployReason.DEPLOYMENT_RULE;
		}

		public NoDeployReason Validate(IEnumerable<Unit> ConvoyOrder)
		{
			if (!Units.All(i => ConvoyOrder.Any(j => i == j || j.Passenger == i))) return NoDeployReason.CONVOY_ORDER;
			if (!Units.Where(i => i.UnitConfiguration.IsPassenger).All(i => i.Carrier != null))
				return NoDeployReason.CONVOY_ORDER;
			return NoDeployReason.NONE;
		}

		public void SetEntryTile(Tile Tile)
		{
			if (Validate(Tile) == NoDeployReason.NONE) _EntryTile = Tile;
		}
	}
}
