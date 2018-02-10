using System;
using System.Collections.Generic;
using System.Linq;
using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public abstract class BaseController : Subcontroller
	{
		protected Pane _Pane;

		protected HumanMatchPlayerController _Controller;

		public BaseController(HumanMatchPlayerController Controller)
		{
			_Controller = Controller;
		}

		public virtual void Clear()
		{
			if (_Pane != null)
			{
				_Controller.RemovePane(_Pane);
				_Pane = null;
			}
		}

		public virtual void Begin()
		{
		}

		public virtual bool Finish()
		{
			return true;
		}

		public virtual void End()
		{
		}

		public abstract void HandleTileLeftClick(Tile Tile);
		public abstract void HandleTileRightClick(Tile Tile);
		public abstract void HandleUnitLeftClick(Unit Unit);
		public abstract void HandleUnitRightClick(Unit Unit);
		public abstract void HandleKeyPress(Keyboard.Key Key);

		public void HandleUnitShiftLeftClick(Unit Unit)
		{
			Clear();
			UnitInfoPane pane = new UnitInfoPane(Unit, _Controller.UnitConfigurationRenderer);
			pane.OnClose += (sender, e) => Clear();
			_Pane = pane;
			_Controller.AddPane(_Pane);
		}

		protected void Recon()
		{
			if (_Controller.SelectedUnit == null) return;

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
				_Pane = pane;
				_Controller.AddPane(_Pane);
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
				if (_Controller.ExecuteOrderAndAlert(order))
				{
					_Controller.SelectUnit(null);
					_Controller.UnHighlight();
				}
			}
			Clear();
		}

		protected void Evacuate()
		{
			if (_Controller.SelectedUnit == null) return;

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
				_Pane = pane;
				_Controller.AddPane(_Pane);
			}
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
				if (_Controller.ExecuteOrderAndAlert(order))
				{
					_Controller.SelectUnit(null);
					_Controller.UnHighlight();
				}
			}
			Clear();
		}

		protected void ClearMinefield()
		{
			if (_Controller.SelectedUnit == null) return;

			Unit onMine =
				_Controller.SelectedUnit.Position.Units.FirstOrDefault(
					i => i.Configuration.UnitClass == UnitClass.MINEFIELD);
			if (onMine != null) ClearMinefield(onMine);
			else
			{
				IEnumerable<Unit> mines =
					_Controller.SelectedUnit.Position.Neighbors()
							   .SelectMany(i => i.Units)
							   .Where(i => i.Configuration.UnitClass == UnitClass.MINEFIELD);
				if (mines.Count() == 1) ClearMinefield(mines.First());
				else if (mines.Count() > 1)
				{
					Clear();
					SelectPane<Unit> pane = new SelectPane<Unit>("Clear Minefield", mines);
					pane.OnItemSelected += ClearMinefield;
					_Pane = pane;
					_Controller.AddPane(_Pane);
				}
			}
		}

		void ClearMinefield(object Sender, ValuedEventArgs<Unit> E)
		{
			ClearMinefield(E.Value);
		}

		void ClearMinefield(Unit Minefield)
		{
			if (_Controller.SelectedUnit != null)
			{
				ClearMinefieldOrder order = new ClearMinefieldOrder(_Controller.SelectedUnit, Minefield);
				if (_Controller.ExecuteOrderAndAlert(order))
				{
					_Controller.SelectUnit(null);
					_Controller.UnHighlight();
				}
			}
			Clear();
		}

		protected void LoadUnit()
		{
			if (_Controller.SelectedUnit == null) return;

			List<Unit> canLoad =
				_Controller.SelectedUnit.Position.Units.Where(
					i => _Controller.SelectedUnit.CanLoad(i) == OrderInvalidReason.NONE).ToList();
			if (canLoad.Count == 1)
			{
				LoadUnit(canLoad.First());
			}
			else if (canLoad.Count > 1)
			{
				Clear();
				SelectPane<Unit> pane = new SelectPane<Unit>("Load Unit", canLoad);
				pane.OnItemSelected += LoadUnit;
				_Pane = pane;
				_Controller.AddPane(_Pane);
			}
		}

		void LoadUnit(object Sender, ValuedEventArgs<Unit> E)
		{
			LoadUnit(E.Value);
		}

		void LoadUnit(Unit Unit)
		{
			if (_Controller.SelectedUnit != null)
			{
				LoadOrder order = new LoadOrder(
					_Controller.SelectedUnit, Unit, _Controller.CurrentTurn.TurnComponent != TurnComponent.DEPLOYMENT);
				if (_Controller.ExecuteOrderAndAlert(order) && order.UseMovement)
				{
					_Controller.SelectUnit(null);
					_Controller.UnHighlight();
				}
			}
			Clear();
		}

		protected void UnloadUnit()
		{
			if (_Controller.SelectedUnit == null) return;

			UnloadOrder order = new UnloadOrder(
				_Controller.SelectedUnit, _Controller.CurrentTurn.TurnComponent != TurnComponent.DEPLOYMENT);
			if (_Controller.ExecuteOrderAndAlert(order) && order.UseMovement)
			{
				_Controller.SelectUnit(null);
				_Controller.UnHighlight();
			}
		}

		protected void Mount()
		{
			if (_Controller.SelectedUnit == null) return;

			MountOrder order = new MountOrder(
				_Controller.SelectedUnit, _Controller.CurrentTurn.TurnComponent != TurnComponent.DEPLOYMENT);
			if (_Controller.ExecuteOrderAndAlert(order) && order.UseMovement)
			{
				_Controller.SelectUnit(null);
				_Controller.UnHighlight();
			}
		}

		protected void Dismount()
		{
			if (_Controller.SelectedUnit == null) return;
			DismountOrder order = new DismountOrder(
				_Controller.SelectedUnit, _Controller.CurrentTurn.TurnComponent != TurnComponent.DEPLOYMENT);
			if (_Controller.ExecuteOrderAndAlert(order) && order.UseMovement)
			{
				_Controller.SelectUnit(null);
				_Controller.UnHighlight();
			}
		}
	}
}
