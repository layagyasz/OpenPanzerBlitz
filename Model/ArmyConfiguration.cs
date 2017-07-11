using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ArmyConfiguration
	{
		private enum Attribute { FACTION, TEAM, DEPLOYMENT_CONFIGURATIONS }

		public readonly Faction Faction;
		public readonly byte Team;
		public readonly List<DeploymentConfiguration> DeploymentConfigurations;

		public ArmyConfiguration(
			Faction Faction, byte Team, IEnumerable<DeploymentConfiguration> DeploymentConfigurations)
		{
			this.Faction = Faction;
			this.Team = Team;
			this.DeploymentConfigurations = DeploymentConfigurations.ToList();
		}

		public ArmyConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			Faction = (Faction)attributes[(int)Attribute.FACTION];
			Team = (byte)attributes[(int)Attribute.TEAM];
			DeploymentConfigurations = ((List<object>)attributes[
				(int)Attribute.DEPLOYMENT_CONFIGURATIONS]).Select(i => (DeploymentConfiguration)i).ToList();
		}
	}
}
