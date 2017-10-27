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

		public MovementController(HumanMatchPlayerController Controller, bool VehicleMovement)
			: base(Controller)
		{
			this.VehicleMovement = VehicleMovement;
		}

		void Clear()
		{
			if (_SelectPane != null)
			{
				_Controller.RemovePane(_SelectPane);
				_SelectPane = null;
			}
		}

		public override void HandleTileLeftClick(Tile Tile)
		{
			Clear();
			if (_Controller.SelectedUnit != null)
			{
				MovementOrder order = new MovementOrder(_Controller.SelectedUnit, Tile, false);
				if (_Controller.Match.ExecuteOrder(order)) SetMovementHighlight(_Controller.SelectedUnit);
				else _Controller.Alert(order.Validate());
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
				&& (Unit.CanMove(VehicleMovement, false) == NoMoveReason.NONE
					|| Unit.CanUnload() == NoUnloadReason.NONE))
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
							HIGHLIGHT_COLORS[
								Math.Min(
									(int)(Math.Ceiling(i.Item3) * 4 / Unit.RemainingMovement),
									HIGHLIGHT_COLORS.Length - 1)])));
			}
			else _Controller.UnHighlight();
		}

		public override void HandleKeyPress(Keyboard.Key Key)
		{
			// Load/Unload
			if (Key == Keyboard.Key.L)
			{
				if (_Controller.SelectedUnit != null)
				{
					List<Unit> canLoad =
						_Controller.SelectedUnit.Position.Units.Where(
							i => _Controller.SelectedUnit.CanLoad(i) == NoLoadReason.NONE).ToList();
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
						_Controller.AddPane(_SelectPane);
					}
				}
			}
			else if (Key == Keyboard.Key.U) UnloadUnit();
			// Recon
			else if (Key == Keyboard.Key.R)
			{
				if (_Controller.SelectedUnit != null)
				{
					List<Direction> directions =
						Enum.GetValues(typeof(Direction))
							.Cast<Direction>()
							.Where(i => _Controller.SelectedUnit.CanExitDirection(i))
							.ToList();
					if (directions.Count == 1) ReconDirection(directions.First());
					else if (directions.Count > 1)
					{
						Clear();
						SelectPane<Direction> pane = new SelectPane<Direction>("Recon", directions);
						pane.OnItemSelected += ReconDirection;
						_SelectPane = pane;
						_Controller.AddPane(_SelectPane);
					}
				}
			}
			// Evacuate
			else if (Key == Keyboard.Key.E)
			{
				if (_Controller.SelectedUnit != null)
				{
					List<Direction> directions =
						Enum.GetValues(typeof(Direction))
							.Cast<Direction>()
							.Where(i => _Controller.SelectedUnit.CanExitDirection(i))
							.ToList();
					if (directions.Count == 1) EvacuateDirection(directions.First());
					else if (directions.Count > 1)
					{
						Clear();
						SelectPane<Direction> pane = new SelectPane<Direction>("Evacuate", directions);
						pane.OnItemSelected += EvacuateDirection;
						_SelectPane = pane;
						_Controller.AddPane(_SelectPane);
					}
				}
			}
			// Mount/Dismount
			else if (Key == Keyboard.Key.M) Mount();
			else if (Key == Keyboard.Key.D) Dismount();
		}

		void LoadUnit(object sender, ValuedEventArgs<Unit> E)
		{
			LoadUnit(E.Value);
		}

		void LoadUnit(Unit Unit)
		{
			if (_Controller.SelectedUnit != null)
			{
				LoadOrder order = new LoadOrder(_Controller.SelectedUnit, Unit);
				if (!_Controller.Match.ExecuteOrder(order)) _Controller.Alert(order.Validate());
				else SetMovementHighlight(_Controller.SelectedUnit);
			}
			Clear();
		}

		void UnloadUnit()
		{
			if (_Controller.SelectedUnit != null)
			{
				UnloadOrder order = new UnloadOrder(_Controller.SelectedUnit);
				if (!_Controller.Match.ExecuteOrder(order)) _Controller.Alert(order.Validate());
				else SetMovementHighlight(_Controller.SelectedUnit);
			}
		}

		void ReconDirection(object Sender, ValuedEventArgs<Direction> E)
		{
			ReconDirection(E.Value);
		}

		void ReconDirection(Direction Direction)
		{
			if (_Controller.SelectedUnit != null)
			{
				ReconOrder order = new ReconOrder(_Controller.SelectedUnit, Direction);
				if (!_Controller.Match.ExecuteOrder(order)) _Controller.Alert(order.Validate());
				else _Controller.UnHighlight();
			}
			Clear();
		}

		void EvacuateDirection(object Sender, ValuedEventArgs<Direction> E)
		{
			EvacuateDirection(E.Value);
		}

		void EvacuateDirection(Direction Direction)
		{
			if (_Controller.SelectedUnit != null)
			{
				EvacuateOrder order = new EvacuateOrder(_Controller.SelectedUnit, Direction);
				if (!_Controller.Match.ExecuteOrder(order)) _Controller.Alert(order.Validate());
				else _Controller.UnHighlight();
			}
			Clear();
		}

		void Mount()
		{
			if (_Controller.SelectedUnit != null)
			{
				MountOrder order = new MountOrder(_Controller.SelectedUnit);
				if (!_Controller.Match.ExecuteOrder(order)) _Controller.Alert(order.Validate());
				else SetMovementHighlight(_Controller.SelectedUnit);
			}
		}

		void Dismount()
		{
			if (_Controller.SelectedUnit != null)
			{
				DismountOrder order = new DismountOrder(_Controller.SelectedUnit);
				if (!_Controller.Match.ExecuteOrder(order)) _Controller.Alert(order.Validate());
				else SetMovementHighlight(_Controller.SelectedUnit);

			}
		}
	}
}
