using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Serialization;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class FormationTemplateAtom : FormationTemplate
	{
		public readonly WeightedVector<UnitConfigurationLock> UnitConfigurationLocks;

		public double ExpectedValue { get; }

		public FormationTemplateAtom(IEnumerable<UnitConfigurationLock> UnitConfigurationLocks)
		{
			this.UnitConfigurationLocks = new WeightedVector<UnitConfigurationLock>();
			foreach (var l in UnitConfigurationLocks) this.UnitConfigurationLocks.Add(l.GetWeight(), l);

			ExpectedValue = HarmonicAverage(UnitConfigurationLocks.Select(i => i.GetValue()));
		}

		public FormationTemplateAtom(ParseBlock Block)
			: this(Block.BreakToList<UnitConfigurationLock>()) { }

		public IEnumerable<Formation> Generate(Random Random)
		{
			yield return new UnitGroup(
				string.Empty,
				new List<UnitCount>
				{
					new UnitCount(
						UnitConfigurationLocks[Random.NextDouble()].UnitConfigurations.First().UnitConfiguration,
						1)
				});
		}

		static double HarmonicAverage(IEnumerable<double> Values)
		{
			return Values.Count() / Values.Sum(i => 1.0 / i);
		}
	}
}
