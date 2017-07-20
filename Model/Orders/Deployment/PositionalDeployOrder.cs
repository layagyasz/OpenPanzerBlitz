using System;
using System.Linq;

namespace PanzerBlitz
{
	public class PositionalDeployOrder : Order
	{
		public readonly Unit Unit;
		public readonly Tile Tile;

		public PositionalDeployOrder(Unit Unit, Tile Tile)
		{
			this.Unit = Unit;
			this.Tile = Tile;
		}

		public NoDeployReason Validate()
		{
			if (Unit.Deployment is PositionalDeployment)
				return ((PositionalDeployment)Unit.Deployment).Validate(Unit, Tile);
			return NoDeployReason.DEPLOYMENT_RULE;
		}

		public bool Execute(Random Random)
		{
			if (Validate() != NoDeployReason.NONE) return false;

			if (Tile == null)
			{
				Unit.Remove();
				Unit.Deployed = false;
			}
			else
			{
				Unit.Place(Tile);
				Unit.Deployed = true;
			}

			return true;
		}
	}
}
