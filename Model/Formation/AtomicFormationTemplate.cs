using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class AtomicFormationTemplate : FormationTemplate
	{
		public readonly List<FormationTemplate> Atoms;

		public double ExpectedValue { get; }

		public AtomicFormationTemplate(IEnumerable<FormationTemplate> Atoms)
		{
			this.Atoms = Atoms.ToList();
			ExpectedValue = Atoms.Sum(i => i.ExpectedValue);
		}

		public AtomicFormationTemplate(ParseBlock Block)
			: this(Block.BreakToList<FormationTemplate>()) { }

		public IEnumerable<Formation> Generate(Random Random)
		{
			yield return new UnitGroup(
				string.Empty,
				new CompositeFormation(string.Empty, Atoms.SelectMany(i => i.Generate(Random))).Flatten());
		}
	}
}
