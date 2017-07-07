using System;
using System.Linq;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class MovementController : Controller
	{
		static readonly Color[] HIGHLIGHT_COLORS = new Color[]
		{
			new Color(255, 0, 0, 120),
			new Color(255, 128, 0, 120),
		  	new Color(255, 255, 0, 120),
			new Color(0, 255, 0, 120)
		};

		public readonly bool VehicleMovement;

		Army _Army;

		Match _Match;
		GameScreen _GameScreen;

		Unit _SelectedUnit;
		Highlight _MoveHighlight;

		public MovementController(bool VehicleMovement, Match Match, GameScreen GameScreen)
		{
			this.VehicleMovement = VehicleMovement;

			_Match = Match;
			_GameScreen = GameScreen;
		}

		public void Begin(Army Army)
		{
			_Army = Army;

			_MoveHighlight = new Highlight();
			_GameScreen.HighlightLayer.AddHighlight(_MoveHighlight);
		}

		public void End()
		{
			_GameScreen.HighlightLayer.RemoveHighlight(_MoveHighlight);
		}

		public void HandleTileLeftClick(Tile Tile)
		{
			if (_SelectedUnit != null)
			{
				MovementOrder order = new MovementOrder(_SelectedUnit, Tile, false);
				if (_Match.ExecuteOrder(order)) SetMovementHighlight(_SelectedUnit);
			}
		}

		public void HandleTileRightClick(Tile Tile)
		{
		}

		public void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Army
				&& Unit.UnitConfiguration.IsVehicle == VehicleMovement
				&& Unit.CanMove(false) == NoMoveReason.NONE)
			{
				_SelectedUnit = Unit;

				SetMovementHighlight(Unit);
			}
		}

		public void HandleUnitRightClick(Unit Unit)
		{
		}

		void SetMovementHighlight(Unit Unit)
		{
			_GameScreen.HighlightLayer.RemoveHighlight(_MoveHighlight);
			if (Unit.RemainingMovement > 0)
			{
				_MoveHighlight = new Highlight(
					Unit.GetFieldOfMovement(false).Select(
						i => new Tuple<Tile, Color>(
							i.Item1,
							HIGHLIGHT_COLORS[
								Math.Min(
									(int)(Math.Ceiling(i.Item3) * 4 / Unit.RemainingMovement),
									HIGHLIGHT_COLORS.Length - 1)])));
			}
			else _MoveHighlight = new Highlight();
			_GameScreen.HighlightLayer.AddHighlight(_MoveHighlight);
		}
	}
}
