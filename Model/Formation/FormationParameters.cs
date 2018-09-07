using System.Collections.Generic;
using System.Linq;

namespace PanzerBlitz
{
	public class FormationParameters
	{
		public readonly ArmyParameters Parameters;
		public readonly bool[] Features;

		public FormationParameters(ArmyParameters Parameters)
			: this(Parameters, new List<FormationFeature>()) { }

		public FormationParameters(ArmyParameters Parameters, IEnumerable<FormationFeature> Features)
		{
			this.Parameters = Parameters;
			this.Features = Utils.ToArray(Features);
		}

		public FormationParameters WithFeatures(IEnumerable<FormationFeature> Features)
		{
			return new FormationParameters(Parameters, Utils.ToValues<FormationFeature>(this.Features).Union(Features));
		}

		public bool Matches(UnitConfigurationLink Link)
		{
			return Parameters.Matches(Link);
		}

		public bool Matches(UnitConstraints Constraints)
		{
			return Parameters.Parameters.Matches(Constraints);
		}
	}
}
