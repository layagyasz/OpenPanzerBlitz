using System;
using System.Collections.Generic;
using Cardamom.Planar;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ZoneDeploymentConfiguration : DeploymentConfiguration
	{
		enum Attribute { DISPLAY_NAME, ZONE }

		public readonly Polygon Zone;
		string _DisplayName;

		public string DisplayName
		{
			get
			{
				return _DisplayName;
			}
		}

		public ZoneDeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Zone = (Polygon)attributes[(int)Attribute.ZONE];
		}

		public Deployment GenerateDeployment(Army Army, IEnumerable<Unit> Units)
		{
			return new ZoneDeployment(Army, Units, this);
		}
	}
}
