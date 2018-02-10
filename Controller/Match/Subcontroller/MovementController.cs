using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Utilities;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class MovementController : BaseController
	{
		public readonly bool VehicleMovement;

		public MovementController(HumanMatchPlayerController Controller, bool VehicleMovement)
			: base(Controller)
		{
			this.VehicleMovement = VehicleMovement;
		}

		public override void HandleTileLeftClick(Tile Tile)
		{
			Clear();
			if (_Controller.SelectedUnit != null)
			{
				MovementOrder order = new MovementOrder(_Controller.SelectedUnit, Tile, false);
				if (_Controller.ExecuteOrderAndAlert(order)) SetMovementHighlight(_Controller.SelectedUnit);
			}
		}

		public override void End()
		{
			base.End();
			Clear();
		}

		public override void HandleTileRightClick(Tile Tile)
		{
			Clear();
		}

		public override void HandleUnitLeftClick(Unit Unit)
		{
			Clear();
			if (Unit.Army == _Controller.CurrentTurn.Army
				&& (Unit.CanMove(VehicleMovement, false) == OrderInvalidReason.NONE
					|| Unit.CanUnload() == OrderInvalidReason.NONE))
			{
				_Controller.SelectUnit(Unit);
				SetMovementHighlight(Unit);
			}
		}

		public override void HandleUnitRightClick(Unit Unit)
		{
			Clear();
		}

		void SetMovementHighlight(Unit Unit)
		{
			if (Unit.RemainingMovement > 0)
			{
				_Controller.Highlight(
					Unit.GetFieldOfMovement(false).Select(
						i => new Tuple<Tile, Color>(
							i.Item1,
							HumanMatchPlayerController.HIGHLIGHT_COLORS[
								Math.Max(0, Math.Min(
									(int)(Math.Ceiling(i.Item3) * 4 / Unit.RemainingMovement),
									HumanMatchPlayerController.HIGHLIGHT_COLORS.Length - 1))])));
			}
			else _Controller.UnHighlight();
		}

		public override void HandleKeyPress(Keyboard.Key Key)
		{
			if (Key == Keyboard.Key.D) Dismount();
			else if (Key == Keyboard.Key.E) Evacuate();
			else if (Key == Keyboard.Key.I) ClearMinefield();
			else if (Key == Keyboard.Key.L) LoadUnit();
			else if (Key == Keyboard.Key.M) Mount();
			else if (Key == Keyboard.Key.R) Recon();
			else if (Key == Keyboard.Key.U) UnloadUnit();
		}
	}
}
