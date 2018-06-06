using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public interface UnitTracker
	{
		Army TrackingArmy { get; set; }

		UnitVisibility GetVisibility(SightFinder SightFinder, Unit Unit);
		List<Tuple<Unit, UnitVisibility>> Update(SightFinder SightFinder, Unit Unit);
		List<Tuple<Unit, UnitVisibility>> Remove(SightFinder SightFinder, Unit Unit);
		List<Tuple<Unit, UnitVisibility>> ComputeDelta(SightFinder SightFinder, Unit Unit, MovementEventArgs Movement);
		List<Tuple<Unit, UnitVisibility>> ComputeDelta(
			SightFinder SightFinder, List<Tuple<Tile, TileSightLevel>> TileDeltas);
	}
}
