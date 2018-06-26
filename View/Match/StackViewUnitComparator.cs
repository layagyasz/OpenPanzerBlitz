using System.Collections.Generic;

namespace PanzerBlitz
{
	public class StackViewUnitComparator : IComparer<UnitView>
	{
		public int Compare(UnitView x, UnitView y)
		{
			if (x.Unit.Carrier == y.Unit) return -1;
			if (x.Unit == y.Unit.Carrier) return 1;
			if (x.Unit.Army == y.Unit.Army)
			{
				if (y.Unit.Configuration.CanLoad(x.Unit.Configuration) == OrderInvalidReason.NONE) return 1;
				if (x.Unit.Configuration.CanLoad(y.Unit.Configuration) == OrderInvalidReason.NONE) return -1;
			}
			if (x.Unit.Covers(y.Unit)) return 1;
			if (y.Unit.Covers(x.Unit)) return -1;
			if (x.Unit.IsCovered()) return -1;
			if (y.Unit.IsCovered()) return 1;
			if (y.Unit.Configuration.IsEmplaceable()) return 1;
			if (x.Unit.Configuration.IsEmplaceable()) return -1;
			if (x.Unit.Configuration.IsAircraft()) return 1;
			if (y.Unit.Configuration.IsAircraft()) return -1;
			return 0;
		}
	}
}
