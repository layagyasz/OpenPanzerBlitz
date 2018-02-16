using System.Collections.Generic;

namespace PanzerBlitz
{
	public class StackViewUnitComparator : IComparer<UnitView>
	{
		public int Compare(UnitView x, UnitView y)
		{
			if (x.Unit.Carrier == y.Unit) return -1;
			if (x.Unit == y.Unit.Carrier) return 1;
			if (y.Unit.Configuration.CanLoad(x.Unit.Configuration) == OrderInvalidReason.NONE) return 1;
			if (x.Unit.Configuration.CanLoad(y.Unit.Configuration) == OrderInvalidReason.NONE) return -1;
			if (x.Unit.Configuration.UnitClass == UnitClass.FORT) return 1;
			if (y.Unit.Configuration.UnitClass == UnitClass.FORT) return -1;
			if (y.Unit.Configuration.UnitClass == UnitClass.BLOCK
				|| y.Unit.Configuration.UnitClass == UnitClass.MINEFIELD
				|| y.Unit.Configuration.UnitClass == UnitClass.BRIDGE) return 1;
			if (x.Unit.Configuration.UnitClass == UnitClass.BLOCK
				|| x.Unit.Configuration.UnitClass == UnitClass.MINEFIELD
				|| x.Unit.Configuration.UnitClass == UnitClass.BRIDGE) return -1;
			return 0;
		}
	}
}
