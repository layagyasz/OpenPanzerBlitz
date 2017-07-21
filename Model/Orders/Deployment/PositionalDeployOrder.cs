using System;
using System.Linq;

namespace PanzerBlitz
{
	public class PositionalDeployOrder : DeployOrder
	{
		public readonly Unit Unit;
		public readonly Tile Tile;

		public override Army Army
		{
			get
			{
				return Unit.Army;
			}
		}

		public PositionalDeployOrder(Unit Unit, Tile Tile)
		{
			this.Unit = Unit;
			this.Tile = Tile;
		}

		public override NoDeployReason Validate()
		{
			if (Unit.Deployment is PositionalDeployment)
				return ((PositionalDeployment)Unit.Deployment).Validate(Unit, Tile);
			return NoDeployReason.DEPLOYMENT_RULE;
		}

		public override bool Execute(Random Random)
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
