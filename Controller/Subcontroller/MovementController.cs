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
						LoadUnit(canLoad.First());
					}
					else if (canLoad.Count > 1)
					{
						_LoadUnitPane = new LoadUnitPane(canLoad);
						_GameScreen.AddPane(_LoadUnitPane);
						_LoadUnitPane.OnUnitSelected += LoadUnit;
					}
				}
			}
			else if (Key == Keyboard.Key.U) UnloadUnit();
		}

		void LoadUnit(object sender, ValueChangedEventArgs<Unit> E)
		{
			LoadUnit(E.Value);
		}

		void LoadUnit(Unit Unit)
		{
			if (_SelectedUnit != null)
			{
				LoadOrder order = new LoadOrder(_SelectedUnit, Unit);
				if (!_Match.ExecuteOrder(order)) _GameScreen.Alert(order.Validate().ToString());
				else SetMovementHighlight(_SelectedUnit);
			}
			Clear();
		}

		void UnloadUnit()
		{
			if (_SelectedUnit != null)
			{
				UnloadOrder order = new UnloadOrder(_SelectedUnit);
				if (!_Match.ExecuteOrder(order)) _GameScreen.Alert(order.Validate().ToString());
				else SetMovementHighlight(_SelectedUnit);
			}
		}
	}
}
