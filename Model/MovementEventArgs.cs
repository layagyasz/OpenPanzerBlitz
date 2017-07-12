using System;
namespace PanzerBlitz
{
	public class MovementEventArgs : EventArgs
	{
		public readonly Tile Tile;

		public MovementEventArgs(Tile Tile)
		{
			this.Tile = Tile;
		}
	}
}
