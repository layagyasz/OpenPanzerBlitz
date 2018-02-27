using System;
using System.Linq;

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
			_Controller.Clear();
			if (_Controller.SelectedUnit != null)
			{
				var order = new MovementOrder(_Controller.SelectedUnit, Tile, false);
				if (_Controller.ExecuteOrderAndAlert(order)) SetMovementHighlight(_Controller.SelectedUnit);
			}
		}

		public override void HandleTileRightClick(Tile Tile)
		{
			_Controller.Clear();
		}

		public override void HandleUnitLeftClick(Unit Unit)
		{
			_Controller.Clear();
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
			_Controller.Clear();
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
			switch (Key)
			{
				case Keyboard.Key.D: _Controller.Dismount(); break;
				case Keyboard.Key.E: _Controller.Evacuate(); break;
				case Keyboard.Key.I: _Controller.ClearMinefield(); break;
				case Keyboard.Key.L: _Controller.LoadUnit(); break;
				case Keyboard.Key.M: _Controller.Mount(); break;
				case Keyboard.Key.P: _Controller.Emplace(); break;
				case Keyboard.Key.R: _Controller.Recon(); break;
				case Keyboard.Key.U: _Controller.UnloadUnit(); break;
			}
		}
	}
}
