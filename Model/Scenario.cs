using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class Scenario
	{
		private enum Attribute { ARMY_CONFIGURATIONS };

		public readonly List<ArmyConfiguration> ArmyConfigurations;
		public readonly bool SimultaneousDeployment;
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
			Map = new Map(100, 100);
		}
	}
}
