using System;
using System.Collections.Generic;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public abstract class BaseController : Controller
	{
		public static readonly Color[] HIGHLIGHT_COLORS = new Color[]
		{
			new Color(0, 255, 0, 120),
			new Color(255, 255, 0, 120),
			new Color(255, 128, 0, 120),
			new Color(255, 0, 0, 120)
		};

		protected Army _Army;
		protected Match _Match;
		protected GameScreen _GameScreen;

		protected Unit _SelectedUnit;

		Highlight _Highlight;

		public BaseController(Match Match, GameScreen GameScreen)
		{
			_Match = Match;
			_GameScreen = GameScreen;
		}

		public virtual void Begin(Army Army)
		{
			_Army = Army;

			_Highlight = new Highlight();
			_GameScreen.HighlightLayer.AddHighlight(_Highlight);
		}

		public virtual void End()
		{
			_GameScreen.HighlightLayer.RemoveHighlight(_Highlight);
		}

		public abstract void HandleTileLeftClick(Tile Tile);
		public abstract void HandleTileRightClick(Tile Tile);
		public abstract void HandleUnitLeftClick(Unit Unit);
		public abstract void HandleUnitRightClick(Unit Unit);
		public abstract void HandleKeyPress(Keyboard.Key Key);

		protected void UnHighlight()
		{
			_GameScreen.HighlightLayer.RemoveHighlight(_Highlight);
			_Highlight = new Highlight();
			_GameScreen.HighlightLayer.AddHighlight(_Highlight);
		}

		protected void Highlight(IEnumerable<Tuple<Tile, Color>> Highlight)
		{
			_GameScreen.HighlightLayer.RemoveHighlight(_Highlight);
			_Highlight = new Highlight(Highlight);
			_GameScreen.HighlightLayer.AddHighlight(_Highlight);
		}
	}
}
