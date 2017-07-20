using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { COORDINATE }

		List<UnitConfiguration> _UnitConfigurations;
		public readonly Coordinate Coordinate;

		public TileDeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Coordinate = (Coordinate)attributes[(int)Attribute.COORDINATE];
		}

		public Deployment GenerateDeployment(IEnumerable<Unit> Units)
		{
			return new TileDeployment(Units, this);
		}	}
}
