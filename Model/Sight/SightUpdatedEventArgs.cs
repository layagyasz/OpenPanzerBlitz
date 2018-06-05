using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class SightUpdatedEventArgs : EventArgs
	{
		public readonly List<Tuple<Tile, TileSightLevel>> TileDeltas;

		public SightUpdatedEventArgs(List<Tuple<Tile, TileSightLevel>> TileDeltas)
		{
			this.TileDeltas = TileDeltas;
		}
	}
}
