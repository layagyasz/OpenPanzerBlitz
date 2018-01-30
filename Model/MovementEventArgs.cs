using System;

using Cardamom.Graphing;

namespace PanzerBlitz
{
	public class MovementEventArgs : EventArgs
	{
		public readonly Tile Tile;
		public readonly Path<Tile> Path;
		public readonly Unit Carrier;

		public MovementEventArgs(Tile Tile, Path<Tile> Path, Unit Carrier)
		{
			this.Tile = Tile;
			this.Path = Path;
			this.Carrier = Carrier;
		}
	}
}
