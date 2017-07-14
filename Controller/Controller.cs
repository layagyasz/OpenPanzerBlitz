using System;

using SFML.Window;

namespace PanzerBlitz
{
	public interface Controller
	{
		void Begin(Army Army);
		void End();
		void HandleTileLeftClick(Tile Tile);
		void HandleTileRightClick(Tile Tile);
		void HandleUnitLeftClick(Unit Unit);
		void HandleUnitRightClick(Unit Unit);
		void HandleKeyPress(Keyboard.Key key);
	}
}
