using System;
using System.Linq;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class AircraftController : BaseAttackController
	{
		public AircraftController(HumanMatchPlayerController Controller)
			: base(Controller) { }

		public override void HandleTileLeftClick(Tile Tile)
		{
			if (_Controller.SelectedUnit != null)
			{
				var order = new MovementOrder(_Controller.SelectedUnit, Tile, false);
				if (_Controller.ExecuteOrderAndAlert(order)) SetAircraftHighlight(_Controller.SelectedUnit);
			}
		}

		public override void HandleTileRightClick(Tile Tile) { }

		public override void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Controller.CurrentTurn.Army
				&& Unit.Configuration.IsAircraft())
			{
				_Controller.SelectUnit(Unit);
				SetAircraftHighlight(Unit);
			}
			else if (Unit.Army != _Controller.CurrentTurn.Army)
			{
				if (_Controller.SelectedUnit != null)
				{
					AddAttack(
						Unit.Position,
						new AirSingleAttackOrder(
							_Controller.SelectedUnit, Unit, _Controller.UseSecondaryWeapon()));
				}
			}
		}

		void SetAircraftHighlight(Unit Unit)
		{
			_Controller.Highlight(Unit.GetFieldOfSight(AttackMethod.AIR).Select(
				i => new Tuple<Tile, Color>(
					i.Final,
					_Controller.GetRangeColor(i, Unit, AttackMethod.AIR))));
		}
	}
}
