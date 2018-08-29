using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ReplicatingFormationTemplate : FormationTemplate
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

		public ReplicatingFormationTemplate(FormationTemplate Template, int Count)
		{
			this.Template = Template;
			this.Count = Count;
		}

		public ReplicatingFormationTemplate(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Template = (FormationTemplate)attributes[(int)Attribute.TEMPLATE];
			Count = (int)(attributes[(int)Attribute.COUNT] ?? 1);
		}

		public bool Matches(ArmyParameters Parameters)
		{
			return Template.Matches(Parameters);
		}

		public IEnumerable<Formation> Generate(Random Random, ArmyParameters Parameters)
		{
			return Enumerable.Repeat(Template.Generate(Random, Parameters), Count).SelectMany(i => i);
		}
	}
}
