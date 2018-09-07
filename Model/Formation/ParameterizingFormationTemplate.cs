using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ParameterizingFormationTemplate : FormationTemplate
	{
		enum Attribute { FEATURES, TEMPLATE }

		public readonly List<FormationFeature> Features;
		public readonly FormationTemplate Template;

		public ParameterizingFormationTemplate(IEnumerable<FormationFeature> Features, FormationTemplate Template)
		{
			this.Features = Features.ToList();
			this.Template = Template;
		}

		public ParameterizingFormationTemplate(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Features = (List<FormationFeature>)attributes[(int)Attribute.FEATURES];
			Template = (FormationTemplate)attributes[(int)Attribute.TEMPLATE];
		}

		public double GetExpectedValue(FormationParameters Parameters)
		{
			return Template.GetExpectedValue(Parameters);
		}

		public bool Matches(FormationParameters Parameters)
		{
			return Template.Matches(Parameters.WithFeatures(Features));
		}

		public IEnumerable<Formation> Generate(Random Random, FormationParameters Parameters)
		{
			return Template.Generate(Random, Parameters.WithFeatures(Features));
		}
	}
}
