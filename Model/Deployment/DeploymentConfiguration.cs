using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface DeploymentConfiguration
	{
		Deployment GenerateDeployment(IEnumerable<Unit> Units);
	}
}
