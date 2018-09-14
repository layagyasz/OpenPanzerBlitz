using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class BindingFormationTemplate : FormationTemplate
	{
		public readonly List<FormationTemplate> Subtemplates;

		public BindingFormationTemplate(IEnumerable<FormationTemplate> Subtemplates)
		{
			this.Subtemplates = Subtemplates.ToList();
		}

		public BindingFormationTemplate(ParseBlock Block)
			: this(Block.BreakToList<FormationTemplate>()) { }

		public double GetExpectedValue(FormationParameters Parameters)
		{
			return Subtemplates.Sum(i => i.GetExpectedValue(Parameters));
		}

		public bool Matches(FormationParameters Parameters)
		{
			return Subtemplates.All(i => i.Matches(Parameters));
		}

		public IEnumerable<Formation> Generate(Random Random, FormationParameters Parameters)
		{
			yield return new CompositeFormation(
				string.Empty,
				Subtemplates.SelectMany(i => i.Generate(Random, Parameters)));
		}

		static double HarmonicAverage(IEnumerable<double> Values)
		{
			return Values.Count() / Values.Sum(i => 1.0 / i);
		}
	}
}
