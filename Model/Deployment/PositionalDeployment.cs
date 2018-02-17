using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class PositionalDeployment : Deployment
	{
		public readonly PositionalDeploymentConfiguration DeploymentConfiguration;

		public override DeploymentConfiguration Configuration
		{
			get
			{
				return DeploymentConfiguration;
			}
		}

		public PositionalDeployment(
			Army Army,
			IEnumerable<Unit> Units,
			PositionalDeploymentConfiguration DeploymentConfiguration,
			IdGenerator IdGenerator)
			: base(Army, Units, IdGenerator)
		{
			this.DeploymentConfiguration = DeploymentConfiguration;
		}

		public override bool AutomateDeployment()
		{
			foreach (Unit u in Units)
			{
				var validTiles = Army.Match.Map.TilesEnumerable.Where(
					i => Validate(u, i) == OrderInvalidReason.NONE).ToList();
				if (validTiles.Count == 1) Army.Match.ExecuteOrder(new PositionalDeployOrder(u, validTiles.First()));
				if (validTiles.Count == 0) throw new Exception("No valid entry tiles for PositionalDeployment.");
			}
			return IsConfigured();
		}

		public override bool IsConfigured()
		{
			return Units.All(i => i.Position != null);
		}

		public override OrderInvalidReason Validate(Unit Unit, Tile Tile)
		{
			var v = base.Validate(Unit, Tile);
			if (v != OrderInvalidReason.NONE) return v;

			if (Tile != null)
			{
				if (DeploymentConfiguration.Matcher != null && !DeploymentConfiguration.Matcher.Matches(Tile))
					return OrderInvalidReason.DEPLOYMENT_RULE;
			}
			return OrderInvalidReason.NONE;
		}
	}
}
