using System;
using System.Linq;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class CombatMoveController : Controller
	{
		static readonly Color[] HIGHLIGHT_COLORS = new Color[]
		{
			new Color(255, 0, 0, 120),
			new Color(255, 128, 0, 120),
		  	new Color(255, 255, 0, 120),
			new Color(0, 255, 0, 120)
		};

		Army _Army;

		Match _Match;
		GameScreen _GameScreen;

		Unit _SelectedUnit;
		Highlight _MoveHighlight = new Highlight();

		public CombatMoveController(Match Match, GameScreen GameScreen)
		{
			_Match = Match;
			_GameScreen = GameScreen;
		}

		public void Begin(Army Army)
		{
			_Army = Army;

			_GameScreen.HighlightLayer.AddHighlight(_MoveHighlight);
		}

		public void End()
		{
			_GameScreen.HighlightLayer.RemoveHighlight(_MoveHighlight);
		}

		public void HandleTileLeftClick(Tile Tile)
		{
		}

		public void HandleTileRightClick(Tile Tile)
		{
		}

		public void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Army)
			{
				_SelectedUnit = Unit;

				_GameScreen.HighlightLayer.RemoveHighlight(_MoveHighlight);
				_MoveHighlight = new Highlight(
					Unit.GetFieldOfMovement(true).Select(
						i => new Tuple<Tile, Color>(
							i.Item1,
							HIGHLIGHT_COLORS[
								Math.Min(
									(int)Math.Ceiling(i.Item3) * 4 / Unit.UnitConfiguration.Movement,
									HIGHLIGHT_COLORS.Length - 1)])));
				_GameScreen.HighlightLayer.AddHighlight(_MoveHighlight);
			}
		}

		public void HandleUnitRightClick(Unit Unit)
		{
		}

		private void StartAttack(Tile Tile)
		{
		}
	}
}
