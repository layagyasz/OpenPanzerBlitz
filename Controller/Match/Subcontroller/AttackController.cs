using System;
using System.Linq;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class AttackController : BaseAttackController
	{
		public AttackController(HumanMatchPlayerController Controller)
			: base(Controller)
		{
		}

		public override void HandleTileLeftClick(Tile Tile)
		{
		}

		public override void HandleTileRightClick(Tile Tile)
		{
		}

		public override void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Controller.CurrentTurn.Army
				&& Unit.CanAttack(AttackMethod.NORMAL_FIRE) == NoSingleAttackReason.NONE)
			{
				_Controller.SelectUnit(Unit);

				_Controller.Highlight(
					Unit.GetFieldOfSight(AttackMethod.NORMAL_FIRE).Select(
						i => new Tuple<Tile, Color>(
							i.Item1.Final,
							GetRangeColor(
								i.Item2 ? HIGHLIGHT_COLORS : DIM_HIGHLIGHT_COLORS,
								i.Item1.Range,
								Unit.Configuration.GetRange(AttackMethod.NORMAL_FIRE)))));
			}
			else if (Unit.Army != _Controller.CurrentTurn.Army)
			{
				if (_Controller.SelectedUnit != null)
				{
					if (_AttackBuilder == null || _AttackBuilder.AttackAt != Unit.Position)
						StartAttack(
							new AttackOrder(_Controller.CurrentTurn.Army, Unit.Position, AttackMethod.NORMAL_FIRE));
					NoSingleAttackReason r = _AttackBuilder.AddAttacker(
						new NormalSingleAttackOrder(_Controller.SelectedUnit, Unit, AttackMethod.NORMAL_FIRE));
					if (r != NoSingleAttackReason.NONE) _Controller.Alert(r);
					_AttackPane.UpdateDescription();
				}
			}
		}
	}
}
