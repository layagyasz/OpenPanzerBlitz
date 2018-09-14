using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class SwitchingFormationTemplate : FormationTemplate
	{
		public readonly List<FormationTemplate> TemplateCases;

		public SwitchingFormationTemplate(IEnumerable<FormationTemplate> TemplateCases)
		{
			this.TemplateCases = TemplateCases.ToList();
		}

		public SwitchingFormationTemplate(ParseBlock Block)
			: this(Block.BreakToList<FormationTemplate>()) { }

		public double GetExpectedValue(FormationParameters Parameters)
		{
			return HarmonicAverage(
				TemplateCases.Where(i => i.Matches(Parameters)).Select(i => i.GetExpectedValue(Parameters)));
		}

		public bool Matches(FormationParameters Parameters)
		{
			return TemplateCases.Any(i => i.Matches(Parameters));
		}

		public IEnumerable<Formation> Generate(Random Random, FormationParameters Parameters)
		{
			var selected = TemplateCases.FirstOrDefault(i => i.Matches(Parameters));
			if (selected != null) return selected.Generate(Random, Parameters);
			return Enumerable.Empty<Formation>();
		}

		static double HarmonicAverage(IEnumerable<double> Values)
		{
			return Values.Count() / Values.Sum(i => 1.0 / i);
		}
	}
}
