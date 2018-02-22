using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class Scenario : Serializable
	{
		enum Attribute
		{
			NAME,
			ENVIRONMENT,
			MAP_CONFIGURATION,
			ARMY_CONFIGURATIONS,
			DEPLOYMENT_ORDER,
			TURN_ORDER,
			TURNS
		};

		public readonly string Name;
		public readonly List<ArmyConfiguration> ArmyConfigurations;
		public readonly List<ArmyConfiguration> DeploymentOrder;
		public readonly List<ArmyConfiguration> TurnOrder;
		public readonly byte Turns;
		public readonly Environment Environment;
		public readonly MapConfiguration MapConfiguration;

		public IEnumerable<UnitConfiguration> UnitConfigurations
		{
			get { return ArmyConfigurations.SelectMany(i => i.UnitConfigurations); }
		}

		public Scenario(
			IEnumerable<ArmyConfiguration> ArmyConfigurations,
			byte Turns,
			Environment Environment,
			MapConfiguration MapConfiguration)
		{
			this.ArmyConfigurations = ArmyConfigurations.ToList();
			DeploymentOrder = this.ArmyConfigurations;
			TurnOrder = this.ArmyConfigurations;
			this.Turns = Turns;
			this.Environment = Environment;
			this.MapConfiguration = MapConfiguration;
		}

		public Scenario(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Name = (string)attributes[(int)Attribute.NAME];
			ArmyConfigurations = (List<ArmyConfiguration>)attributes[(int)Attribute.ARMY_CONFIGURATIONS];

			var deploymentOrderIndices = (byte[])attributes[(int)Attribute.DEPLOYMENT_ORDER];
			var turnOrderIndices = (byte[])attributes[(int)Attribute.TURN_ORDER];
			Turns = (byte)attributes[(int)Attribute.TURNS];
			DeploymentOrder = deploymentOrderIndices.Select(i => ArmyConfigurations[i]).ToList();
			TurnOrder = turnOrderIndices.Select(i => ArmyConfigurations[i]).ToList();

			Environment = (Environment)attributes[(int)Attribute.ENVIRONMENT];
			MapConfiguration = (MapConfiguration)attributes[(int)Attribute.MAP_CONFIGURATION];
		}

		public Scenario(SerializationInputStream Stream)
		{
			Name = Stream.ReadString();
			ArmyConfigurations = Stream.ReadEnumerable(i => new ArmyConfiguration(Stream)).ToList();
			DeploymentOrder = Stream.ReadEnumerable(i => ArmyConfigurations[Stream.ReadByte()]).ToList();
			TurnOrder = Stream.ReadEnumerable(i => ArmyConfigurations[Stream.ReadByte()]).ToList();
			Turns = Stream.ReadByte();
			Environment = GameData.Environments[Stream.ReadString()];
			MapConfiguration = (MapConfiguration)MapConfigurationSerializer.Instance.Deserialize(Stream);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Name);
			Stream.Write(ArmyConfigurations);
			Stream.Write(DeploymentOrder, i => Stream.Write((byte)ArmyConfigurations.IndexOf(i)));
			Stream.Write(TurnOrder, i => Stream.Write((byte)ArmyConfigurations.IndexOf(i)));
			Stream.Write(Turns);
			Stream.Write(Environment.UniqueKey);
			MapConfigurationSerializer.Instance.Serialize(MapConfiguration, Stream);
		}
	}
}
