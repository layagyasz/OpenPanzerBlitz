using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class FilteringFormationTemplate
	{
		enum Attribute { TEMPLATE, CONSTRAINTS };

		public readonly FormationTemplate Template;
		public readonly UnitConstraints Constraints;

		public double ExpectedValue
		{
			get
			{
				return Template.ExpectedValue;
			}
		}

		public FilteringFormationTemplate(FormationTemplate Template, UnitConstraints Constraints)
		{
			this.Template = Template;
			this.Constraints = Constraints;
		}

		public FilteringFormationTemplate(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Template = (FormationTemplate)attributes[(int)Attribute.TEMPLATE];
			Constraints = (UnitConstraints)attributes[(int)Attribute.CONSTRAINTS];
		}

		public bool Matches(ArmyParameters Parameters)
		{
			return Parameters.Parameters.Matches(Constraints) && Template.Matches(Parameters);
		}

		public IEnumerable<Formation> Generate(Random Random, ArmyParameters Parameters)
		{
			return Template.Generate(Random, Parameters);
		}
	}
}
