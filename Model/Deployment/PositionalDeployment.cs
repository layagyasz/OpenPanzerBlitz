using System;
using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public abstract class PositionalDeployment : Deployment
	{
		public PositionalDeployment(Army Army, IEnumerable<Unit> Units, IdGenerator IdGenerator)
			: base(Army, Units, IdGenerator)
		{
		}

		public override bool IsConfigured()
		{
			return Units.All(i => i.Deployed && Validate(i, i.Position) == NoDeployReason.NONE);
		}
	}
}
