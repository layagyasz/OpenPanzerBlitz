﻿using System;

using SFML.Window;

namespace PanzerBlitz
{
	public interface Subcontroller
	{
		void Begin(Army Army);
		bool Finish();
		void End();
		void HandleTileLeftClick(Tile Tile);
		void HandleTileRightClick(Tile Tile);
		void HandleUnitLeftClick(Unit Unit);
		void HandleUnitRightClick(Unit Unit);
		void HandleKeyPress(Keyboard.Key key);
	}
}