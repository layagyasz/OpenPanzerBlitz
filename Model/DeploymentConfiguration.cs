using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface DeploymentConfiguration
	{
		DeploymentType DeploymentType { get; }
		Deployment GenerateDeployment(IEnumerable<Unit> Units);
	}
}
