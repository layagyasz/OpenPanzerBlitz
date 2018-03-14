using System;
using System.Linq;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class AttackController : BaseAttackController
	{
		public AttackController(HumanMatchPlayerController Controller)
			: base(Controller) { }

		public override void HandleTileLeftClick(Tile Tile) { }

		public override void HandleTileRightClick(Tile Tile) { }

		public override void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Controller.CurrentTurn.Army
				&& Unit.CanAttack(AttackMethod.NORMAL_FIRE) == OrderInvalidReason.NONE)
			{
				_Controller.SelectUnit(Unit);

				_Controller.Highlight(
					Unit.GetFieldOfSight(AttackMethod.NORMAL_FIRE).Select(
						i => new Tuple<Tile, Color>(i.Item1.Final, _Controller.GetRangeColor(i.Item1, Unit, i.Item2))));
			}
			else if (Unit.Army != _Controller.CurrentTurn.Army)
			{
				if (_Controller.SelectedUnit != null)
				{
					if (Unit.Configuration.IsAircraft())
					{
						AddAttack(
							Unit.Position,
							new AntiAirSingleAttackOrder(
								_Controller.SelectedUnit, Unit.Position, _Controller.UseSecondaryWeapon()));
					}
					else
					{
						AddAttack(
							Unit.Position,
							new NormalSingleAttackOrder(
								_Controller.SelectedUnit, Unit, _Controller.UseSecondaryWeapon()));
					}
				}
			}
		}
	}
}
