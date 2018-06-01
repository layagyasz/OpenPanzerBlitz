using System;
using System.Linq;

using SFML.Graphics;
using SFML.Window;

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
			HandleClick(Unit, AttackMethod.DIRECT_FIRE);
		}

		public override void HandleUnitShiftLeftClick(Unit Unit)
		{
			HandleClick(Unit, AttackMethod.INDIRECT_FIRE);
		}

		void HandleClick(Unit Unit, AttackMethod AttackMethod)
		{
			if (Unit.Army == _Controller.CurrentTurn.Army && Unit.CanAttack(AttackMethod) == OrderInvalidReason.NONE)
			{
				_Controller.SelectUnit(Unit);

				_Controller.Highlight(
					Unit.GetFieldOfSight(AttackMethod).Select(
						i => new Tuple<Tile, Color>(i.Final, _Controller.GetRangeColor(i, Unit))));
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
						if (AttackMethod == AttackMethod.INDIRECT_FIRE)
						{
							AddAttack(
								Unit.Position,
								new IndirectFireSingleAttackOrder(
									_Controller.SelectedUnit, Unit.Position, _Controller.UseSecondaryWeapon()));
						}
						else
						{
							AddAttack(
								Unit.Position,
								new DirectFireSingleAttackOrder(
									_Controller.SelectedUnit, Unit, _Controller.UseSecondaryWeapon()));
						}
					}
				}
			}
		}
	}
}
