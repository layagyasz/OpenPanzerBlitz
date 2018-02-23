using System;
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

		protected BaseController(HumanMatchPlayerController Controller)
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
			var pane = new UnitInfoPane(Unit, _Controller.UnitConfigurationRenderer);
			pane.OnClose += (sender, e) => Clear();
			_Pane = pane;
			_Controller.AddPane(_Pane);
		}

		protected void Recon()
		{
			if (_Controller.SelectedUnit == null) return;

			var directions =
				Enum.GetValues(typeof(Direction))
					.Cast<Direction>()
					.Where(i => _Controller.SelectedUnit.CanExitDirection(i))
					.ToList();
			if (directions.Count == 1) ReconDirection(directions.First());
			else if (directions.Count > 1)
			{
				Clear();
				var pane = new SelectPane<Direction>("Recon", directions);
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
				var order = new ReconOrder(_Controller.SelectedUnit, Direction);
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

			var directions =
				Enum.GetValues(typeof(Direction))
					.Cast<Direction>()
					.Where(i => _Controller.SelectedUnit.CanExitDirection(i))
					.ToList();
			if (directions.Count == 1) EvacuateDirection(directions.First());
			else if (directions.Count > 1)
			{
				Clear();
				var pane = new SelectPane<Direction>("Evacuate", directions);
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
				var order = new EvacuateOrder(_Controller.SelectedUnit, Direction);
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

			var onMine =
				_Controller.SelectedUnit.Position.Units.FirstOrDefault(
					i => i.Configuration.UnitClass == UnitClass.MINEFIELD);
			if (onMine != null) ClearMinefield(onMine);
			else
			{
				var mines =
					_Controller.SelectedUnit.Position.Neighbors()
							   .SelectMany(i => i.Units)
							   .Where(i => i.Configuration.UnitClass == UnitClass.MINEFIELD);
				if (mines.Count() == 1) ClearMinefield(mines.First());
				else if (mines.Count() > 1)
				{
					Clear();
					var pane = new SelectPane<Unit>("Clear Minefield", mines);
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
				var order = new ClearMinefieldOrder(_Controller.SelectedUnit, Minefield);
				if (_Controller.ExecuteOrderAndAlert(order))
				{
					_Controller.SelectUnit(null);
					_Controller.UnHighlight();
				}
			}
			Clear();
		}

		protected void Emplace()
		{
			if (_Controller.SelectedUnit == null) return;

			var emplaceables =
				_Controller.SelectedUnit.Position.Neighbors()
						   .SelectMany(i => i.Units)
						   .Where(i => i.Configuration.Emplaceable());
			if (emplaceables.Count() == 1) Emplace(emplaceables.First());
			else if (emplaceables.Count() > 1)
			{
				Clear();
				var pane = new SelectPane<Unit>("Emplace Unit", emplaceables);
				pane.OnItemSelected += Emplace;
				_Pane = pane;
				_Controller.AddPane(_Pane);
			}
		}

		void Emplace(object Sender, ValuedEventArgs<Unit> E)
		{
			Emplace(E.Value);
		}

		void Emplace(Unit Target)
		{
			if (_Controller.SelectedUnit != null)
			{
				var order = new EmplaceOrder(_Controller.SelectedUnit, Target);
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

			var canLoad =
				_Controller.SelectedUnit.Position.Units.Where(
					i => _Controller.SelectedUnit.CanLoad(i) == OrderInvalidReason.NONE).ToList();
			if (canLoad.Count == 1)
			{
				LoadUnit(canLoad.First());
			}
			else if (canLoad.Count > 1)
			{
				Clear();
				var pane = new SelectPane<Unit>("Load Unit", canLoad);
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
				var order = new LoadOrder(
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

			var order = new UnloadOrder(
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

			var order = new MountOrder(
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
			var order = new DismountOrder(
				_Controller.SelectedUnit, _Controller.CurrentTurn.TurnComponent != TurnComponent.DEPLOYMENT);
			if (_Controller.ExecuteOrderAndAlert(order) && order.UseMovement)
			{
				_Controller.SelectUnit(null);
				_Controller.UnHighlight();
			}
		}
	}
}
