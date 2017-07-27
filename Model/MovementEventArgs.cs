using System;

using Cardamom.Graphing;

namespace PanzerBlitz
{
	public class MovementEventArgs : EventArgs
	{
		public readonly Tile Tile;
		public readonly Path<Tile> Path;

		public MovementEventArgs(Tile Tile, Path<Tile> Path)
		{
			this.Tile = Tile;
			this.Path = Path;
		}
	}
}
