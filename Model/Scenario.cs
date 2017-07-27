using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class Scenario
	{
		enum Attribute { NAME, MAP_CONFIGURATION, ARMY_CONFIGURATIONS, DEPLOYMENT_ORDER, TURNS };

		public readonly string Name;
		public readonly List<ArmyConfiguration> ArmyConfigurations;
		public readonly List<ArmyConfiguration> DeploymentOrder;
		public readonly byte Turns;
		public readonly MapConfiguration MapConfiguration;

		public Scenario(IEnumerable<ArmyConfiguration> ArmyConfigurations)
		{
			this.ArmyConfigurations = ArmyConfigurations.ToList();
		}

		public Scenario(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			Name = (string)attributes[(int)Attribute.NAME];
			ArmyConfigurations = (List<ArmyConfiguration>)attributes[(int)Attribute.ARMY_CONFIGURATIONS];

			byte[] deploymentOrderIndices = (byte[])attributes[(int)Attribute.DEPLOYMENT_ORDER];
			Turns = (byte)attributes[(int)Attribute.TURNS];
			DeploymentOrder = deploymentOrderIndices.Select(i => ArmyConfigurations[i]).ToList();

			MapConfiguration = (MapConfiguration)attributes[(int)Attribute.MAP_CONFIGURATION];
		}
	}
}
