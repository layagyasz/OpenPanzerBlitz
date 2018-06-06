using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class SightUpdatedEventArgs : EventArgs
	{
		public readonly Unit Unit;
		public readonly MovementEventArgs Movement;
		public readonly List<Tuple<Tile, TileSightLevel>> TileDeltas;
		public readonly List<Tuple<Unit, UnitVisibility>> UnitDeltas;

		public SightUpdatedEventArgs(
			Unit Unit,
			MovementEventArgs Movement,
			List<Tuple<Tile, TileSightLevel>> TileDeltas,
			List<Tuple<Unit, UnitVisibility>> UnitDeltas)
		{
			this.Unit = Unit;
			this.Movement = Movement;
			this.TileDeltas = TileDeltas;
			this.UnitDeltas = UnitDeltas;
		}
	}
}
