using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class Scenario : Serializable
	{
		enum Attribute { NAME, MAP_CONFIGURATION, ARMY_CONFIGURATIONS, DEPLOYMENT_ORDER, TURN_ORDER, TURNS };

		public readonly string Name;
		public readonly List<ArmyConfiguration> ArmyConfigurations;
		public readonly List<ArmyConfiguration> DeploymentOrder;
		public readonly List<ArmyConfiguration> TurnOrder;
		public readonly byte Turns;
		public readonly BoardCompositeMapConfiguration MapConfiguration;

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
			byte[] turnOrderIndices = (byte[])attributes[(int)Attribute.TURN_ORDER];
			Turns = (byte)attributes[(int)Attribute.TURNS];
			DeploymentOrder = deploymentOrderIndices.Select(i => ArmyConfigurations[i]).ToList();
			TurnOrder = turnOrderIndices.Select(i => ArmyConfigurations[i]).ToList();

			MapConfiguration = (BoardCompositeMapConfiguration)attributes[(int)Attribute.MAP_CONFIGURATION];
		}

		public Scenario(SerializationInputStream Stream)
		{
			Name = Stream.ReadString();
			ArmyConfigurations = Stream.ReadEnumerable(i => new ArmyConfiguration(Stream)).ToList();
			DeploymentOrder = Stream.ReadEnumerable(i => ArmyConfigurations[Stream.ReadByte()]).ToList();
			TurnOrder = Stream.ReadEnumerable(i => ArmyConfigurations[Stream.ReadByte()]).ToList();
			Turns = Stream.ReadByte();
			MapConfiguration = new BoardCompositeMapConfiguration(Stream);
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Name);
			Stream.Write(ArmyConfigurations);
			Stream.Write(DeploymentOrder, i => Stream.Write((byte)ArmyConfigurations.IndexOf(i)));
			Stream.Write(TurnOrder, i => Stream.Write((byte)ArmyConfigurations.IndexOf(i)));
			Stream.Write(Turns);
			Stream.Write(MapConfiguration);
		}
	}
}
