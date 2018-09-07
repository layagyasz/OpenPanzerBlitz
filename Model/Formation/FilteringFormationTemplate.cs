using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class FilteringFormationTemplate : FormationTemplate
	{
		enum Attribute { TEMPLATE, CONSTRAINTS, FEATURES };

		public readonly FormationTemplate Template;
		public readonly UnitConstraints Constraints;
		public readonly List<FormationFeature> Features;

		public FilteringFormationTemplate(
			FormationTemplate Template, UnitConstraints Constraints, IEnumerable<FormationFeature> Features)
		{
			this.Template = Template;
			this.Constraints = Constraints;
			this.Features = Features.ToList();
		}

		public FilteringFormationTemplate(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Template = (FormationTemplate)attributes[(int)Attribute.TEMPLATE];
			Constraints = (UnitConstraints)(attributes[(int)Attribute.CONSTRAINTS] ?? new UnitConstraints());
			Features = (List<FormationFeature>)(attributes[(int)Attribute.FEATURES] ?? new List<FormationFeature>());
		}

		public double GetExpectedValue(FormationParameters Parameters)
		{
			return Template.GetExpectedValue(Parameters);
		}

		public bool Matches(FormationParameters Parameters)
		{
			return Parameters.Matches(Constraints)
							 && (Features.Count == 0 || Features.Any(i => Parameters.Features[(int)i]))
							 && Template.Matches(Parameters);
		}

		public IEnumerable<Formation> Generate(Random Random, FormationParameters Parameters)
		{
			return Template.Generate(Random, Parameters);
		}
	}
}
