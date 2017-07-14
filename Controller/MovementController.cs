using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class MovementController : BaseController
	{
		public readonly bool VehicleMovement;

		public MovementController(bool VehicleMovement, Match Match, GameScreen GameScreen)
			: base(Match, GameScreen)
		{
			this.VehicleMovement = VehicleMovement;
		}

		public override void HandleTileLeftClick(Tile Tile)
		{
			if (_SelectedUnit != null)
			{
				MovementOrder order = new MovementOrder(_SelectedUnit, Tile, false);
				if (_Match.ExecuteOrder(order)) SetMovementHighlight(_SelectedUnit);
			}
		}

		public override void HandleTileRightClick(Tile Tile)
		{
		}

		public override void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Army
				&& (Unit.CanMove(VehicleMovement, false) == NoMoveReason.NONE
					|| Unit.CanUnload() == NoUnloadReason.NONE))
			{
				_SelectedUnit = Unit;

				SetMovementHighlight(Unit);
			}
		}

		public override void HandleUnitRightClick(Unit Unit)
		{
		}

		void SetMovementHighlight(Unit Unit)
		{
			if (Unit.RemainingMovement > 0)
			{
				Highlight(
					Unit.GetFieldOfMovement(false).Select(
						i => new Tuple<Tile, Color>(
							i.Item1,
							HIGHLIGHT_COLORS[
								Math.Min(
									(int)(Math.Ceiling(i.Item3) * 4 / Unit.RemainingMovement),
									HIGHLIGHT_COLORS.Length - 1)])));
			}
			else UnHighlight();
		}

		public override void HandleKeyPress(Keyboard.Key Key)
		{
			if (Key == Keyboard.Key.L)
			{
				if (_SelectedUnit != null)
				{
					List<Unit> canLoad =
						_SelectedUnit.Position.Units.Where(i => _SelectedUnit.CanLoad(i) == NoLoadReason.NONE).ToList();
					if (canLoad.Count == 1) _Match.ExecuteOrder(new LoadOrder(_SelectedUnit, canLoad.First()));
					else if (canLoad.Count > 1)
					{
					}
				}
			}
			else if (Key == Keyboard.Key.U)
			{
				if (_SelectedUnit != null) _Match.ExecuteOrder(new UnloadOrder(_SelectedUnit));
			}
		}
	}
}
