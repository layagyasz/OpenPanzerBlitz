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

		public string ToString(int Depth)
		{
			string separator = "\n".PadRight(Depth + 2);
			return separator + string.Join(separator, Subformations.Select(i => i.ToString(Depth + 1)));
		}

		public override string ToString()
		{
			return string.Format("[CompositeFormation: Name={0}]{1}", Name, ToString(0));
		}
	}
}
