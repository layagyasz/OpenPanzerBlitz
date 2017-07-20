using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public abstract class Deployment
	{
		public readonly List<Unit> Units;

		public Deployment(IEnumerable<Unit> Units)
		{
			this.Units = Units.ToList();
		}

		public abstract bool AutomateDeployment(Match Match);
		public abstract bool IsConfigured();

		public abstract string GetDisplayString();
	}
}
