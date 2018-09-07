using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class SelectingFormationTemplate : FormationTemplate
	{
		public readonly List<FormationTemplate> Subtemplates;

		public SelectingFormationTemplate(IEnumerable<FormationTemplate> Subtemplates)
		{
			this.Subtemplates = Subtemplates.ToList();
		}

		public SelectingFormationTemplate(ParseBlock Block)
			: this(Block.BreakToList<FormationTemplate>()) { }

		public double GetExpectedValue(FormationParameters Parameters)
		{
			return HarmonicAverage(
				Subtemplates.Where(i => i.Matches(Parameters)).Select(i => i.GetExpectedValue(Parameters)));
		}

		public bool Matches(FormationParameters Parameters)
		{
			return Subtemplates.Any(i => i.Matches(Parameters));
		}

		public IEnumerable<Formation> Generate(Random Random, FormationParameters Parameters)
		{
			WeightedVector<FormationTemplate> choices = new WeightedVector<FormationTemplate>();
			foreach (var template in Subtemplates.Where(i => i.Matches(Parameters)))
				choices.Add(1 / template.GetExpectedValue(Parameters), template);
			if (choices.Length > 0) return choices[Random.NextDouble()].Generate(Random, Parameters);
			return Enumerable.Empty<Formation>();
		}

		static double HarmonicAverage(IEnumerable<double> Values)
		{
			return Values.Count() / Values.Sum(i => 1.0 / i);
		}
	}
}
