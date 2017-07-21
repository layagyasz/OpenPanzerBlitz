using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public abstract class Deployment
	{
		public readonly Army Army;
		public readonly List<Unit> Units;
		public abstract DeploymentConfiguration Configuration { get; }

		public Deployment(Army Army, IEnumerable<Unit> Units)
		{
			this.Army = Army;
			this.Units = Units.ToList();
		}

		public abstract bool AutomateDeployment(Match Match);
		public abstract bool IsConfigured();
	}
}
