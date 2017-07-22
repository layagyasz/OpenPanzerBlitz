using System;
using System.Linq;

namespace PanzerBlitz
{
	public class MovementDeployOrder : DeployOrder
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

		public MovementDeployOrder(Unit Unit, Tile Tile)
		{
			this.Unit = Unit;
			this.Tile = Tile;
		}

		public override NoDeployReason Validate()
		{
			if (Unit.Deployment is ConvoyDeployment)
				return ((ConvoyDeployment)Unit.Deployment).Validate(Unit, Tile);
			return NoDeployReason.DEPLOYMENT_RULE;
		}

		public override bool Execute(Random Random)
		{
			if (Validate() == NoDeployReason.NONE)
			{
				Unit.Place(Tile);
				Unit.Deployed = true;
				return true;
			}
			return false;
		}
	}
}
