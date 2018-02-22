using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface DeploymentConfiguration : Serializable
	{
		UnitGroup UnitGroup { get; }
		Deployment GenerateDeployment(Army Army, IdGenerator IdGenerator);
	}
}
