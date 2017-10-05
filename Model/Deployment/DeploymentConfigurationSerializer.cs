using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public static class DeploymentConfigurationSerializer
	{
		static readonly Type[] DEPLOYMENT_CONFIGURATION_TYPES =
		{
			typeof(ConvoyDeploymentConfiguration),
			typeof(PositionalDeploymentConfiguration)
		};

		static readonly Func<SerializationInputStream, DeploymentConfiguration>[] DESERIALIZERS =
		{
			i => new ConvoyDeploymentConfiguration(i),
			i => new PositionalDeploymentConfiguration(i)
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
