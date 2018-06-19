using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ArmyConfiguration : Serializable
	{
		enum Attribute { FACTION, TEAM, DEPLOYMENT_CONFIGURATIONS, VICTORY_CONDITION }

		public readonly string UniqueKey;
		public readonly Faction Faction;
		public readonly byte Team;
		public readonly List<DeploymentConfiguration> DeploymentConfigurations;
		public readonly VictoryCondition VictoryCondition;

		public IEnumerable<UnitConfiguration> UnitConfigurations
		{
			get
			{
				return DeploymentConfigurations.SelectMany(
					i => i.UnitGroup.UnitCounts.SelectMany(j => j.UnitConfiguration.RepresentedConfigurations));
			}
		}

		public ArmyConfiguration(
			string UniqueKey,
			Faction Faction,
			byte Team,
			IEnumerable<DeploymentConfiguration> DeploymentConfigurations,
			VictoryCondition VictoryCondition)
		{
			this.UniqueKey = UniqueKey;
			this.Faction = Faction;
			this.Team = Team;
			this.DeploymentConfigurations = DeploymentConfigurations.ToList();
			this.VictoryCondition = VictoryCondition;
		}

		public ArmyConfiguration(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			UniqueKey = Block.Name;
			Faction = (Faction)attributes[(int)Attribute.FACTION];
			Team = (byte)attributes[(int)Attribute.TEAM];
			DeploymentConfigurations = ((List<object>)attributes[(int)Attribute.DEPLOYMENT_CONFIGURATIONS])
				.Select(i => (DeploymentConfiguration)i)
				.ToList();
			VictoryCondition = (VictoryCondition)attributes[(int)Attribute.VICTORY_CONDITION];
		}

		public ArmyConfiguration(SerializationInputStream Stream)
		{
			UniqueKey = Stream.ReadString();
			Faction = GameData.Factions[Stream.ReadString()];
			Team = Stream.ReadByte();
			DeploymentConfigurations = Stream.ReadEnumerable(DeploymentConfigurationSerializer.Deserialize).ToList();
			VictoryCondition = new VictoryCondition(Stream);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(UniqueKey);
			Stream.Write(Faction.UniqueKey);
			Stream.Write(Team);
			Stream.Write(DeploymentConfigurations, i => DeploymentConfigurationSerializer.Serialize(i, Stream));
			Stream.Write(VictoryCondition);
		}

		public IEnumerable<UnitConfiguration> BuildUnitConfigurationList()
		{
			return DeploymentConfigurations
				.SelectMany(i => i.UnitGroup.UnitCounts)
				.SelectMany(i => Enumerable.Repeat(i.UnitConfiguration, i.Count));
		}
	}
}
