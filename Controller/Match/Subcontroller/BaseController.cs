using System;
using System.Collections.Generic;
using System.Linq;
using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public abstract class BaseController : Subcontroller
	{
		public static readonly Color[] HIGHLIGHT_COLORS =
		{
			new Color(0, 255, 0, 120),
			new Color(255, 255, 0, 120),
			new Color(255, 128, 0, 120),
			new Color(255, 0, 0, 120)
		};

		public static readonly Color[] DIM_HIGHLIGHT_COLORS =
		{
			new Color(0, 255, 0, 120),
			new Color(255, 255, 0, 120),
			new Color(255, 128, 0, 120),
			new Color(255, 0, 0, 120)
		};

		protected Pane _SelectPane;

		protected HumanMatchPlayerController _Controller;

		public BaseController(HumanMatchPlayerController Controller)
		{
			_Controller = Controller;
		}

		public virtual void Clear()
		{
			if (_SelectPane != null)
			{
				_Controller.RemovePane(_SelectPane);
				_SelectPane = null;
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

		protected void LoadUnit()
		{
			if (_Controller.SelectedUnit != null)
			{
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
					_SelectPane = pane;
					_Controller.AddPane(_SelectPane);
				}
			}
		}

		void LoadUnit(object sender, ValuedEventArgs<Unit> E)
		{
			LoadUnit(E.Value);
		}

		void LoadUnit(Unit Unit)
		{
			if (_Controller.SelectedUnit != null)
			{
				LoadOrder order = new LoadOrder(
					_Controller.SelectedUnit, Unit, _Controller.CurrentTurn.TurnComponent != TurnComponent.DEPLOYMENT);
				if (_Controller.ExecuteOrderAndAlert(order) && order.UseMovement) _Controller.SelectUnit(null);
			}
			Clear();
		}

		protected void UnloadUnit()
		{
			if (_Controller.SelectedUnit != null)
			{
				UnloadOrder order = new UnloadOrder(
					_Controller.SelectedUnit, _Controller.CurrentTurn.TurnComponent != TurnComponent.DEPLOYMENT);
				if (_Controller.ExecuteOrderAndAlert(order) && order.UseMovement) _Controller.SelectUnit(null);
			}
		}

		protected void Mount()
		{
			if (_Controller.SelectedUnit != null)
			{
				MountOrder order = new MountOrder(
					_Controller.SelectedUnit, _Controller.CurrentTurn.TurnComponent != TurnComponent.DEPLOYMENT);
				if (_Controller.ExecuteOrderAndAlert(order) && order.UseMovement) _Controller.SelectUnit(null);
			}
		}

		protected void Dismount()
		{
			if (_Controller.SelectedUnit != null)
			{
				DismountOrder order = new DismountOrder(
					_Controller.SelectedUnit, _Controller.CurrentTurn.TurnComponent != TurnComponent.DEPLOYMENT);
				if (_Controller.ExecuteOrderAndAlert(order) && order.UseMovement) _Controller.SelectUnit(null);
			}
		}
	}
}
