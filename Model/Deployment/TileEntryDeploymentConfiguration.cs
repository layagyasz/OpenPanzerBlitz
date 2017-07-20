using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class TileEntryDeploymentConfiguration : DeploymentConfiguration
	{
		public TileEntryDeploymentConfiguration()
		{
		}

		public Deployment GenerateDeployment(IEnumerable<Unit> Units)
		{
			return new TileEntryDeployment(Units);
		}
	}
}
