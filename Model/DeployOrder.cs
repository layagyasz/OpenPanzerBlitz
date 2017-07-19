using System;
using System.Linq;

namespace PanzerBlitz
{
	public class DeployOrder : Order
	{
		public readonly Unit Unit;
		public readonly Tile Tile;

		public DeployOrder(Unit Unit, Tile Tile)
		{
			this.Unit = Unit;
			this.Tile = Tile;
		}

		public NoDeployReason Validate()
		{
			return Unit.Deployment.Validate(Unit, Tile);
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
