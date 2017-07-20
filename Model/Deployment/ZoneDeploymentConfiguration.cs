using System;
using System.Collections.Generic;
using Cardamom.Planar;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ZoneDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { UNIT_CONFIGURATIONS, ZONE }

		public readonly Polygon Zone;

		public ZoneDeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Zone = (Polygon)attributes[(int)Attribute.ZONE];
		}

		public Deployment GenerateDeployment(IEnumerable<Unit> Units)
		{
			return new ZoneDeployment(Units, this);
		}
	}
}
