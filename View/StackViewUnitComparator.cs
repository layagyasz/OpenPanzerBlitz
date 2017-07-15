using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class StackViewUnitComparator : IComparer<UnitView>
	{
		public int Compare(UnitView x, UnitView y)
		{
			if (x.Unit.Carrier == y.Unit) return -1;
			if (x.Unit == y.Unit.Carrier) return 1;
			if (y.Unit.UnitConfiguration.CanLoad(x.Unit.UnitConfiguration) == NoLoadReason.NONE) return 1;
			if (x.Unit.UnitConfiguration.CanLoad(y.Unit.UnitConfiguration) == NoLoadReason.NONE) return -1;
			if (x.Unit.UnitConfiguration.UnitClass == UnitClass.FORT) return 1;
			if (x.Unit.UnitConfiguration.UnitClass == UnitClass.BLOCK
				|| x.Unit.UnitConfiguration.UnitClass == UnitClass.MINEFIELD
				|| x.Unit.UnitConfiguration.UnitClass == UnitClass.BRIDGE) return -1;
			else return 0;
		}
	}
}
