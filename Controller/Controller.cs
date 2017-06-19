using System;
namespace PanzerBlitz
{
	public interface Controller
	{
		void Begin(Army Army);
		void HandleTileLeftClick(Tile Tile);
		void HandleTileRightClick(Tile Tile);
		void HandleUnitLeftClick(Unit Unit);
		void HandleUnitRightClick(Unit Unit);
	}
}
