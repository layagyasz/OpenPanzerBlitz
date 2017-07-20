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
		public readonly List<Tuple<List<UnitConfiguration>, DeploymentConfiguration>> DeploymentConfigurations;

		public ArmyConfiguration(
			Faction Faction,
			byte Team,
			IEnumerable<Tuple<List<UnitConfiguration>, DeploymentConfiguration>> DeploymentConfigurations)
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
			DeploymentConfigurations = ((List<Tuple<object, object>>)attributes[
				(int)Attribute.DEPLOYMENT_CONFIGURATIONS])
				.Select(i => new Tuple<List<UnitConfiguration>, DeploymentConfiguration>(
					BuildUnitConfigurationList(((List<Tuple<object, object>>)i.Item1)
											   .Select(
												   j => new Tuple<UnitConfiguration, int>(
													   (UnitConfiguration)j.Item1,
													   (int)j.Item2)))
					.ToList(), (DeploymentConfiguration)i.Item2))
				.ToList();
		}

		IEnumerable<UnitConfiguration> BuildUnitConfigurationList(
			IEnumerable<Tuple<UnitConfiguration, int>> UnitCountPairs)
		{
			return UnitCountPairs.SelectMany(i => Enumerable.Repeat(i.Item1, i.Item2));
		}
	}
}
