using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class OmniscientUnitTracker : UnitTracker
	{
		public Army TrackingArmy { get; set; }

		public UnitVisibility GetVisibility(SightFinder SightFinder, Unit Unit)
		{
			return new UnitVisibility(true, Unit.Position);
		}

		public List<Tuple<Unit, UnitVisibility>> Update(SightFinder SightFinder, Unit Unit)
		{
			return new List<Tuple<Unit, UnitVisibility>>
			{
				new Tuple<Unit, UnitVisibility>(Unit, new UnitVisibility(true, Unit.Position))
			};
		}

		public List<Tuple<Unit, UnitVisibility>> Remove(SightFinder SightFinder, Unit Unit)
		{
			return new List<Tuple<Unit, UnitVisibility>>
			{
				new Tuple<Unit, UnitVisibility>(Unit, new UnitVisibility(false, null))
			};
		}

		public List<Tuple<Unit, UnitVisibility>> ComputeDelta(
			SightFinder SightFinder, Unit Unit, MovementEventArgs Movement)
		{
			return new List<Tuple<Unit, UnitVisibility>>();
		}

		public List<Tuple<Unit, UnitVisibility>> ComputeDelta(
			SightFinder SightFinder, List<Tuple<Tile, TileSightLevel>> TileDeltas)
		{
			return new List<Tuple<Unit, UnitVisibility>>();
		}
	}
}
