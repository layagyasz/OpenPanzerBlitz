using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class CompositeFormation : Formation
	{
		public readonly List<Formation> Subformations;

		public string Name { get; }

		public CompositeFormation(string Name, IEnumerable<Formation> Subformations)
		{
			this.Name = Name;
			this.Subformations = Subformations.ToList();
		}

		public IEnumerable<UnitCount> Flatten()
		{
			return Subformations
				.SelectMany(i => i.Flatten())
				.GroupBy(i => i.UnitConfiguration)
				.Select(i => new UnitCount(i.Key, i.Sum(j => j.Count)));
		}
	}
}
