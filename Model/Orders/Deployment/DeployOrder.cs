using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public abstract class DeployOrder : Order
	{
		public abstract void Serialize(SerializationOutputStream Stream);
		public abstract Army Army { get; }
		public abstract NoDeployReason Validate();
		public abstract bool Execute(Random Random);
	}
}
