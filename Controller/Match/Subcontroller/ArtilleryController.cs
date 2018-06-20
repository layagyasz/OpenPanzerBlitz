using System;
using System.Linq;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class ArtilleryController : BaseAttackController
	{
		public ArtilleryController(HumanMatchPlayerController Controller)
			: base(Controller) { }

		public override void HandleTileLeftClick(Tile Tile)
		{
			if (_Controller.SelectedUnit != null)
			{
				if (_Controller.SelectedUnit.Target == Tile)
				{
					AddAttack(
						Tile,
						new IndirectFireSingleAttackOrder(
							_Controller.SelectedUnit, Tile, _Controller.UseSecondaryWeapon()));
				}
				else
				{
					_Controller.ExecuteOrderAndAlert(new TargetOrder(_Controller.SelectedUnit, Tile));
					_Controller.Clear();
				}
			}
		}

		public override void HandleTileRightClick(Tile Tile) { }

		public override void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Controller.CurrentTurn.Army
				&& Unit.CanAttack(AttackMethod.INDIRECT_FIRE) == OrderInvalidReason.NONE)
			{
				_Controller.SelectUnit(Unit);
				_Controller.Highlight(
					Unit.GetFieldOfSight(AttackMethod.INDIRECT_FIRE).Select(
						i => new Tuple<Tile, Color>(
							i.Final,
							i.Final == Unit.Target
								? HumanMatchPlayerController.ACCENT_COLOR
								: _Controller.GetRangeColor(i, Unit, AttackMethod.INDIRECT_FIRE))));
			}
		}
	}
}
