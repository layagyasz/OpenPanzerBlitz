using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class MovementController : BaseController
	{
		public readonly bool VehicleMovement;

		Pane _SelectPane;

		public MovementController(bool VehicleMovement, MatchAdapter Match, MatchScreen GameScreen)
			: base(Match, GameScreen)
		{
			this.VehicleMovement = VehicleMovement;
		}

		void Clear()
		{
			if (_SelectPane != null)
			{
				_GameScreen.PaneLayer.Remove(_SelectPane);
				_SelectPane = null;
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
			// Load/Unload
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
						Clear();
						SelectPane<Unit> pane = new SelectPane<Unit>("Load Unit", canLoad); ;
						pane.OnItemSelected += LoadUnit;
						_SelectPane = pane;
						_GameScreen.PaneLayer.Add(_SelectPane);
					}
				}
			}
			else if (Key == Keyboard.Key.U) UnloadUnit();
			// Recon
			else if (Key == Keyboard.Key.R)
			{
				if (_SelectedUnit != null)
				{
					List<Direction> directions =
						Enum.GetValues(typeof(Direction))
							.Cast<Direction>()
							.Where(i => _SelectedUnit.CanExitDirection(i))
							.ToList();
					if (directions.Count == 1) ReconDirection(directions.First());
					else if (directions.Count > 1)
					{
						Clear();
						SelectPane<Direction> pane = new SelectPane<Direction>("Recon", directions);
						pane.OnItemSelected += ReconDirection;
						_SelectPane = pane;
						_GameScreen.PaneLayer.Add(_SelectPane);
					}
				}
			}
			// Evacuate
			else if (Key == Keyboard.Key.E)
			{
				if (_SelectedUnit != null)
				{
					List<Direction> directions =
						Enum.GetValues(typeof(Direction))
							.Cast<Direction>()
							.Where(i => _SelectedUnit.CanExitDirection(i))
							.ToList();
					if (directions.Count == 1) EvacuateDirection(directions.First());
					else if (directions.Count > 1)
					{
						Clear();
						SelectPane<Direction> pane = new SelectPane<Direction>("Evacuate", directions);
						pane.OnItemSelected += EvacuateDirection;
						_SelectPane = pane;
						_GameScreen.PaneLayer.Add(_SelectPane);
					}
				}
			}
		}

		void LoadUnit(object sender, ValuedEventArgs<Unit> E)
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

		void ReconDirection(object Sender, ValuedEventArgs<Direction> E)
		{
			ReconDirection(E.Value);
		}

		void ReconDirection(Direction Direction)
		{
			if (_SelectedUnit != null)
			{
				ReconOrder order = new ReconOrder(_SelectedUnit, Direction);
				if (!_Match.ExecuteOrder(order)) _GameScreen.Alert(order.Validate().ToString());
				else UnHighlight();
			}
			Clear();
		}

		void EvacuateDirection(object Sender, ValuedEventArgs<Direction> E)
		{
			EvacuateDirection(E.Value);
		}

		void EvacuateDirection(Direction Direction)
		{
			if (_SelectedUnit != null)
			{
				EvacuateOrder order = new EvacuateOrder(_SelectedUnit, Direction);
				if (!_Match.ExecuteOrder(order)) _GameScreen.Alert(order.Validate().ToString());
				else UnHighlight();
			}
			Clear();
		}
	}
}
