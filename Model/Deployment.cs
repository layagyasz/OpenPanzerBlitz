using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class Deployment
	{
		public readonly DeploymentConfiguration DeploymentConfiguration;
		public readonly List<Unit> Units;

		public Deployment(Army Army, DeploymentConfiguration DeploymentConfiguration)
		{
			this.DeploymentConfiguration = DeploymentConfiguration;
			this.Units = DeploymentConfiguration.UnitConfigurations.Select(i => new Unit(Army, i)).ToList();
		}

		public bool Validate(Unit Unit, Tile Tile)
		{
			return DeploymentConfiguration.ValidatorFunction(Unit, Tile);
		}

		public bool Validate()
		{
			return Units.All(i => Validate(i, i.Position));
		}
	}
}
