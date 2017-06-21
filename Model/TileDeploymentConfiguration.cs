using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { UNIT_CONFIGURATIONS, COORDINATE }

		List<UnitConfiguration> _UnitConfigurations;
		public readonly Coordinate Coordinate;

		public IEnumerable<UnitConfiguration> UnitConfigurations
		{
			get
			{
				return _UnitConfigurations;
			}
		}

		public TileDeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			_UnitConfigurations = (List<UnitConfiguration>)attributes[(int)Attribute.UNIT_CONFIGURATIONS];
			Coordinate = (Coordinate)attributes[(int)Attribute.COORDINATE];
		}

		public Deployment GenerateDeployment(Army Army)
		{
			return new TileDeployment(Army, this);
		}	}
}
