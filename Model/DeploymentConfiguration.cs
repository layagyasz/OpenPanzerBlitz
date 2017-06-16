using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class DeploymentConfiguration
	{
		private enum Attribute { UNIT_CONFIGURATIONS, VALIDATOR_FUNCTION }

		public readonly List<UnitConfiguration> UnitConfigurations;
		public readonly Func<Unit, Tile, bool> ValidatorFunction;

		public DeploymentConfiguration(IEnumerable<UnitConfiguration> Units, Func<Unit, Tile, bool> ValidatorFunction)
		{
			this.UnitConfigurations = Units.ToList();
			this.ValidatorFunction = ValidatorFunction;
		}

		public DeploymentConfiguration(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			UnitConfigurations = (List<UnitConfiguration>)attributes[(int)Attribute.UNIT_CONFIGURATIONS];
			ValidatorFunction = (i, j) => true;
		}
	}
}
