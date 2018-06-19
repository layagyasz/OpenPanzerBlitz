using System;
using System.Linq;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class ArtilleryController : BaseAttackController
	{
		public ArtilleryController(HumanMatchPlayerController Controller)
			: base(Controller) { }

		public override void HandleTileLeftClick(Tile Tile) { }

		public override void HandleTileRightClick(Tile Tile) { }

		public override void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Controller.CurrentTurn.Army
				&& Unit.CanAttack(AttackMethod.INDIRECT_FIRE) == OrderInvalidReason.NONE)
				SelectUnit(Unit, AttackMethod.INDIRECT_FIRE);
			else if (Unit.Army != _Controller.CurrentTurn.Army)
			{
				if (_Controller.SelectedUnit != null)
				{
					AddAttack(
						Unit.Position,
						new IndirectFireSingleAttackOrder(
							_Controller.SelectedUnit, Unit.Position, _Controller.UseSecondaryWeapon()));
				}
			}
		}
	}
}
