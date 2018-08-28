using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class LinkingFormationTemplate : FormationTemplate
	{
		enum Attribute { TEMPLATE, COUNT };

		public readonly FormationTemplate Template;
		public readonly int Count;

		public double ExpectedValue
		{
			get
			{
				return Template.ExpectedValue;
			}
		}

		public LinkingFormationTemplate(FormationTemplate Template, int Count)
		{
			this.Template = Template;
			this.Count = Count;
		}

		public LinkingFormationTemplate(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Template = (FormationTemplate)attributes[(int)Attribute.TEMPLATE];
			Count = (int)(attributes[(int)Attribute.COUNT] ?? 1);
		}

		public IEnumerable<Formation> Generate(Random Random)
		{
			return Enumerable.Repeat(Template.Generate(Random), Count).SelectMany(i => i);
		}
	}
}
