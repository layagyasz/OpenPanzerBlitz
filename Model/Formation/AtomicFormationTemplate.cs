using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class AtomicFormationTemplate : FormationTemplate
	{
		public readonly List<FormationTemplate> Atoms;

		public AtomicFormationTemplate(IEnumerable<FormationTemplate> Atoms)
		{
			this.Atoms = Atoms.ToList();
		}

		public AtomicFormationTemplate(ParseBlock Block)
			: this(Block.BreakToList<FormationTemplate>()) { }

		public double GetExpectedValue(FormationParameters Parameters)
		{
			return Atoms.Sum(i => i.GetExpectedValue(Parameters));
		}

		public bool Matches(FormationParameters Parameters)
		{
			return Atoms.Any(i => i.Matches(Parameters));
		}

		public IEnumerable<Formation> Generate(Random Random, FormationParameters Parameters)
		{
			yield return new UnitGroup(
				string.Empty,
				new CompositeFormation(
					string.Empty,
					Atoms.Where(i => i.Matches(Parameters)).SelectMany(i => i.Generate(Random, Parameters))).Flatten());
		}
	}
}
