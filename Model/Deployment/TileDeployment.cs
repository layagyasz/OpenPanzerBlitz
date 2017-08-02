using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class TileDeployment : PositionalDeployment
	{
		public readonly TileDeploymentConfiguration DeploymentConfiguration;

		public override DeploymentConfiguration Configuration
		{
			get
			{
				return DeploymentConfiguration;
			}
		}

		public TileDeployment(
			Army Army,
			IEnumerable<Unit> Units,
			TileDeploymentConfiguration DeploymentConfiguration,
			IdGenerator IdGenerator)
			: base(Army, Units, IdGenerator)
		{
			this.DeploymentConfiguration = DeploymentConfiguration;
		}

		public override bool AutomateDeployment(Match Match)
		{
			bool done = Units.All(i => Match.ExecuteOrder(
				new PositionalDeployOrder(
					i,
					Match.Map.Tiles[
						DeploymentConfiguration.Coordinate.X, DeploymentConfiguration.Coordinate.Y])));
			if (!done) throw new Exception("Deployment order rejected for TileDeployment.");
			return done;
		}

		public override void AutomateMovement(Match Match, bool Vehicle)
		{
		}

		public override NoDeployReason Validate(Unit Unit, Tile Tile)
		{
			NoDeployReason v = base.Validate(Unit, Tile);
			if (v != NoDeployReason.NONE) return v;

			if (Tile != null)
			{
				if (DeploymentConfiguration.Coordinate.X != Tile.Coordinate.X
					|| DeploymentConfiguration.Coordinate.Y != Tile.Coordinate.Y)
					return NoDeployReason.DEPLOYMENT_RULE;
				else return NoDeployReason.NONE;
			}
			else return NoDeployReason.DEPLOYMENT_RULE;
		}
	}
}
