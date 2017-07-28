using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public abstract class PositionalDeployment : Deployment
	{
		public PositionalDeployment(Army Army, IEnumerable<Unit> Units)
			: base(Army, Units)
		{
		}

		public override bool IsConfigured()
		{
			return Units.All(i => i.Position != null);
		}
	}
}
