using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class TileDeployment : Deployment
	{
		public readonly TileDeploymentConfiguration DeploymentConfiguration;

		public TileDeployment(IEnumerable<Unit> Units, TileDeploymentConfiguration DeploymentConfiguration)
			: base(Units, DeploymentConfiguration)
		{
			this.DeploymentConfiguration = DeploymentConfiguration;
		}

		public override bool AutomateDeployment(Match Match)
		{
			bool done = Units.All(i => Match.ExecuteOrder(
				new DeployOrder(
					i,
					Match.Map.Tiles[
						DeploymentConfiguration.Coordinate.X, DeploymentConfiguration.Coordinate.Y])));
			if (!done) throw new Exception("Deployment order rejected for TileDeployment.");
			return done;
		}

		public override bool IsConfigured()
		{
			return Units.All(i => i.Position != null && Validate(i, i.Position) == NoDeployReason.NONE);
		}

		public override NoDeployReason Validate(Unit Unit, Tile Tile)
		{
			NoDeployReason v = base.Validate(Unit, Tile);
			if (v != NoDeployReason.NONE) return v;

			if (Tile != null)
			{
				if (DeploymentConfiguration.Coordinate.X != Tile.X || DeploymentConfiguration.Coordinate.Y != Tile.Y)
					return NoDeployReason.DEPLOYMENT_RULE;
				else return NoDeployReason.NONE;
			}
			else return NoDeployReason.DEPLOYMENT_RULE;
		}
	}
}
