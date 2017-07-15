using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class MovementController : BaseController
	{
		public readonly bool VehicleMovement;

		LoadUnitPane _LoadUnitPane;

		public MovementController(bool VehicleMovement, Match Match, GameScreen GameScreen)
			: base(Match, GameScreen)
		{
			this.VehicleMovement = VehicleMovement;
		}

		void Clear()
		{
			if (_LoadUnitPane != null)
			{
				_GameScreen.RemovePane(_LoadUnitPane);
				_LoadUnitPane = null;
			}
		}

		public override void HandleTileLeftClick(Tile Tile)
		{
			Clear();
			if (_SelectedUnit != null)
			{
				MovementOrder order = new MovementOrder(_SelectedUnit, Tile, false);
				if (_Match.ExecuteOrder(order)) SetMovementHighlight(_SelectedUnit);
				else _GameScreen.Alert(order.Validate().ToString());
			}
		}

		public override void HandleTileRightClick(Tile Tile)
		{
			Clear();
		}

		public override void HandleUnitLeftClick(Unit Unit)
		{
			Clear();
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
			Clear();
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
					if (canLoad.Count == 1)
					{
						_Match.ExecuteOrder(new LoadOrder(_SelectedUnit, canLoad.First()));
						SetMovementHighlight(_SelectedUnit);
					}
					else if (canLoad.Count > 1)
					{
						_LoadUnitPane = new LoadUnitPane(canLoad);
						_GameScreen.AddPane(_LoadUnitPane);
						_LoadUnitPane.OnUnitSelected += LoadUnit;
					}
				}
			}
			else if (Key == Keyboard.Key.U)
			{
				if (_SelectedUnit != null) _Match.ExecuteOrder(new UnloadOrder(_SelectedUnit));
			}
		}

		void LoadUnit(object sender, ValueChangedEventArgs<Unit> E)
		{
			if (_SelectedUnit != null) _Match.ExecuteOrder(new LoadOrder(_SelectedUnit, E.Value));
			SetMovementHighlight(_SelectedUnit);
			Clear();
		}
	}
}
