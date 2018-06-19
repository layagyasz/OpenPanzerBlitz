using System;
using System.Linq;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class AntiAircraftController : BaseAttackController
	{
		public AntiAircraftController(HumanMatchPlayerController Controller)
			: base(Controller) { }

		public override void HandleTileLeftClick(Tile Tile) { }

		public override void HandleTileRightClick(Tile Tile) { }

		public override void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Controller.CurrentTurn.Army
				&& Unit.CanAttack(AttackMethod.ANTI_AIRCRAFT) == OrderInvalidReason.NONE)
				SelectUnit(Unit, AttackMethod.ANTI_AIRCRAFT);
			else if (Unit.Army != _Controller.CurrentTurn.Army)
			{
				if (_Controller.SelectedUnit != null)
				{
					AddAttack(
						Unit.Position,
						new AntiAirSingleAttackOrder(
							_Controller.SelectedUnit, Unit, _Controller.UseSecondaryWeapon()));
				}
			}
		}
	}
}
