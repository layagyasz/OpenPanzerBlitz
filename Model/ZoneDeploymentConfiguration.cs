using System;
using System.Collections.Generic;
using Cardamom.Planar;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ZoneDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { UNIT_CONFIGURATIONS, ZONE }

		List<UnitConfiguration> _UnitConfigurations;
		public readonly Polygon Zone;

		public IEnumerable<UnitConfiguration> UnitConfigurations
		{
			get
			{
				return _UnitConfigurations;
			}
		}

		public ZoneDeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			_UnitConfigurations = (List<UnitConfiguration>)attributes[(int)Attribute.UNIT_CONFIGURATIONS];
			Zone = (Polygon)attributes[(int)Attribute.ZONE];
		}

		public Deployment GenerateDeployment(Army Army)
		{
			return new ZoneDeployment(Army, this);
		}
	}
}
