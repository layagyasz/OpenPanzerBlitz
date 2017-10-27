using System;
using System.Collections.Generic;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public abstract class BaseController : Subcontroller
	{
		public static readonly Color[] HIGHLIGHT_COLORS =
		{
			new Color(0, 255, 0, 120),
			new Color(255, 255, 0, 120),
			new Color(255, 128, 0, 120),
			new Color(255, 0, 0, 120)
		};

		public static readonly Color[] DIM_HIGHLIGHT_COLORS =
		{
			new Color(0, 255, 0, 120),
			new Color(255, 255, 0, 120),
			new Color(255, 128, 0, 120),
			new Color(255, 0, 0, 120)
		};

		protected HumanMatchPlayerController _Controller;

		public BaseController(HumanMatchPlayerController Controller)
		{
			_Controller = Controller;
		}

		public virtual void Begin()
		{
		}

		public virtual bool Finish()
		{
			return true;
		}

		public virtual void End()
		{
		}

		public abstract void HandleTileLeftClick(Tile Tile);
		public abstract void HandleTileRightClick(Tile Tile);
		public abstract void HandleUnitLeftClick(Unit Unit);
		public abstract void HandleUnitRightClick(Unit Unit);
		public abstract void HandleKeyPress(Keyboard.Key Key);
	}
}
