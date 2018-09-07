using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class ReplicatingFormationTemplate : FormationTemplate
	{
		enum Attribute { TEMPLATE, COUNT, REGENERATE };

		public readonly FormationTemplate Template;
		public readonly int Count;
		public readonly bool Regenerate;

		public ReplicatingFormationTemplate(FormationTemplate Template, int Count, bool Regenerate)
		{
			this.Template = Template;
			this.Count = Count;
			this.Regenerate = Regenerate;
		}

		public ReplicatingFormationTemplate(ParseBlock Block)
		{
			var attributes = Block.BreakToAttributes<object>(typeof(Attribute));

			Template = (FormationTemplate)attributes[(int)Attribute.TEMPLATE];
			Count = (int)(attributes[(int)Attribute.COUNT] ?? 1);
			Regenerate = (bool)(attributes[(int)Attribute.REGENERATE] ?? false);
		}

		public double GetExpectedValue(FormationParameters Parameters)
		{
			return Count * Template.GetExpectedValue(Parameters);
		}

		public bool Matches(FormationParameters Parameters)
		{
			return Template.Matches(Parameters);
		}

		public IEnumerable<Formation> Generate(Random Random, FormationParameters Parameters)
		{
			if (Regenerate) return Enumerable.Repeat(Template, Count).SelectMany(i => i.Generate(Random, Parameters));
			return Enumerable.Repeat(Template.Generate(Random, Parameters).ToList(), Count).SelectMany(i => i);
		}
	}
}
