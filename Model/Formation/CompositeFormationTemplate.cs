using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class CompositeFormationTemplate : FormationTemplate
	{
		public readonly List<FormationTemplate> Subtemplates;

		public double ExpectedValue { get; }

		public CompositeFormationTemplate(IEnumerable<FormationTemplate> Subtemplates)
		{
			this.Subtemplates = Subtemplates.ToList();
			ExpectedValue = HarmonicAverage(Subtemplates.Select(i => i.ExpectedValue));
		}

		public CompositeFormationTemplate(ParseBlock Block)
			: this(Block.BreakToList<FormationTemplate>()) { }

		public bool Matches(ArmyParameters Parameters)
		{
			return true;
		}

		public IEnumerable<Formation> Generate(Random Random, ArmyParameters Parameters)
		{
			yield return new CompositeFormation(
				string.Empty,
				Subtemplates.Where(i => i.Matches(Parameters)).SelectMany(i => i.Generate(Random, Parameters)));
		}

		static double HarmonicAverage(IEnumerable<double> Values)
		{
			return Values.Count() / Values.Sum(i => 1.0 / i);
		}
	}
}
