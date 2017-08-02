using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface DeploymentConfiguration
	{
		string DisplayName { get; }

		Deployment GenerateDeployment(Army Army, IEnumerable<Unit> Units, IdGenerator IdGenerator);
	}
}
