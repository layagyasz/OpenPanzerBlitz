using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ArmyConfiguration
	{
		private enum Attribute { FACTION, DEPLOYMENT_CONFIGURATIONS }

		public readonly Faction Faction;
		public readonly List<DeploymentConfiguration> DeploymentConfigurations;

		public ArmyConfiguration(Faction Faction, IEnumerable<DeploymentConfiguration> DeploymentConfigurations)
		{
			this.Faction = Faction;
			this.DeploymentConfigurations = DeploymentConfigurations.ToList();
		}

		public ArmyConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			Faction = (Faction)attributes[(int)Attribute.FACTION];
			DeploymentConfigurations = ((List<object>)attributes[
				(int)Attribute.DEPLOYMENT_CONFIGURATIONS]).Select(i => (DeploymentConfiguration)i).ToList();
		}
	}
}
