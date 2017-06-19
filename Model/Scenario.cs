using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class Scenario
	{
		private enum Attribute { ARMY_CONFIGURATIONS, DEPLOYMENT_ORDER, TURNS };

		public readonly List<ArmyConfiguration> ArmyConfigurations;
		public readonly List<ArmyConfiguration> DeploymentOrder;
		public readonly byte Turns;
		public readonly Map Map;

		public Scenario(IEnumerable<ArmyConfiguration> ArmyConfigurations)
		{
			this.ArmyConfigurations = ArmyConfigurations.ToList();
		}

		public Scenario(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			ArmyConfigurations = (List<ArmyConfiguration>)attributes[(int)Attribute.ARMY_CONFIGURATIONS];

			byte[] deploymentOrderIndices = (byte[])attributes[(int)Attribute.DEPLOYMENT_ORDER];
			DeploymentOrder = deploymentOrderIndices.Select(i => ArmyConfigurations[i]).ToList();

			Map = new Map(100, 100);
		}
	}
}
