using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ArmyConfiguration : Serializable
	{
		private enum Attribute { FACTION, TEAM, DEPLOYMENT_CONFIGURATIONS, VICTORY_CONDITION }

		public readonly Faction Faction;
		public readonly byte Team;
		public readonly List<Tuple<List<UnitConfiguration>, DeploymentConfiguration>> DeploymentConfigurations;
		public readonly VictoryCondition VictoryCondition;

		public ArmyConfiguration(
			Faction Faction,
			byte Team,
			IEnumerable<Tuple<List<UnitConfiguration>, DeploymentConfiguration>> DeploymentConfigurations,
			VictoryCondition VictoryCondition)
		{
			this.Faction = Faction;
			this.Team = Team;
			this.DeploymentConfigurations = DeploymentConfigurations.ToList();
			this.VictoryCondition = VictoryCondition;
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
			VictoryCondition = (VictoryCondition)attributes[(int)Attribute.VICTORY_CONDITION];
		}

		public ArmyConfiguration(SerializationInputStream Stream)
		{
			Faction = GameData.Factions[Stream.ReadString()];
			Team = Stream.ReadByte();
			DeploymentConfigurations = Stream.ReadEnumerable(i =>
			{
				return new Tuple<List<UnitConfiguration>, DeploymentConfiguration>(
					Stream.ReadEnumerable(j => GameData.UnitConfigurations[Stream.ReadString()]).ToList(),
					DeploymentConfigurationSerializer.Deserialize(Stream));
			}).ToList();
			VictoryCondition = new VictoryCondition(Stream);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Faction.UniqueKey);
			Stream.Write(Team);
			Stream.Write(DeploymentConfigurations, i =>
			{
				Stream.Write(i.Item1, j => Stream.Write(j.UniqueKey));
				DeploymentConfigurationSerializer.Serialize(i.Item2, Stream);
			});
			Stream.Write(VictoryCondition);
		}

		IEnumerable<UnitConfiguration> BuildUnitConfigurationList(
			IEnumerable<Tuple<UnitConfiguration, int>> UnitCountPairs)
		{
			return UnitCountPairs.SelectMany(i => Enumerable.Repeat(i.Item1, i.Item2));
		}
	}
}
