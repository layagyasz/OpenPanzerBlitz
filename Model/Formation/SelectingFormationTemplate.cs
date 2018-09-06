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

		public double ExpectedValue { get; }

		public SelectingFormationTemplate(IEnumerable<FormationTemplate> Subtemplates)
		{
			this.Subtemplates = Subtemplates.ToList();
			ExpectedValue = HarmonicAverage(Subtemplates.Select(i => i.ExpectedValue));
		}

		public SelectingFormationTemplate(ParseBlock Block)
			: this(Block.BreakToList<FormationTemplate>()) { }

		public bool Matches(ArmyParameters Parameters)
		{
			return Subtemplates.Any(i => i.Matches(Parameters));
		}

		public IEnumerable<Formation> Generate(Random Random, ArmyParameters Parameters)
		{
			WeightedVector<FormationTemplate> choices = new WeightedVector<FormationTemplate>();
			foreach (var template in Subtemplates.Where(i => i.Matches(Parameters)))
			{
				Console.WriteLine(1 / template.ExpectedValue);
				choices.Add(1 / template.ExpectedValue, template);
			}
			return choices[Random.NextDouble()].Generate(Random, Parameters);
		}

		static double HarmonicAverage(IEnumerable<double> Values)
		{
			return Values.Count() / Values.Sum(i => 1.0 / i);
		}
	}
}
