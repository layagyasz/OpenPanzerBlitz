using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public static class DeploymentConfigurationSerializer
	{
		static readonly Type[] DEPLOYMENT_CONFIGURATION_TYPES =
		{
			typeof(OneOfZoneDeploymentConfiguration),
			typeof(TileDeploymentConfiguration),
			typeof(TileEntryDeploymentConfiguration),
			typeof(ZoneDeploymentConfiguration)
		};

		static readonly Func<SerializationInputStream, DeploymentConfiguration>[] DESERIALIZERS =
		{
			i => new OneOfZoneDeploymentConfiguration(i),
			i => new TileDeploymentConfiguration(i),
			i => new TileEntryDeploymentConfiguration(i),
			i => new ZoneDeploymentConfiguration(i)
		};

		public static void Serialize(DeploymentConfiguration DeploymentConfiguration, SerializationOutputStream Stream)
		{
			Stream.Write((byte)Array.IndexOf(DEPLOYMENT_CONFIGURATION_TYPES, DeploymentConfiguration.GetType()));
			Stream.Write(DeploymentConfiguration);
		}

		public static DeploymentConfiguration Deserialize(SerializationInputStream Stream)
		{
			return DESERIALIZERS[Stream.ReadByte()](Stream);
		}
	}
}
