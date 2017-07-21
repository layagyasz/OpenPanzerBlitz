using System;
namespace PanzerBlitz
{
	public abstract class DeployOrder : Order
	{
		public abstract Army Army { get; }
		public abstract NoDeployReason Validate();
		public abstract bool Execute(Random Random);
	}
}
