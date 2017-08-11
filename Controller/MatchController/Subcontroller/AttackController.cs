using System;
using System.Linq;

using SFML.Graphics;
using SFML.Window;

namespace PanzerBlitz
{
	public class AttackController : BaseAttackController
	{
		public AttackController(MatchAdapter Match, GameScreen GameScreen)
			: base(Match, GameScreen)
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
			if (Unit.Army == _Army && Unit.CanAttack(AttackMethod.NORMAL_FIRE) == NoSingleAttackReason.NONE)
			{
				_SelectedUnit = Unit;

				Highlight(
					Unit.GetFieldOfSight(AttackMethod.NORMAL_FIRE).Select(
						i => new Tuple<Tile, Color>(
							i.Final,
							HIGHLIGHT_COLORS[
								Math.Min(
									i.Range * HIGHLIGHT_COLORS.Length
									/ (Unit.Configuration.GetRange(AttackMethod.NORMAL_FIRE) + 1),
									HIGHLIGHT_COLORS.Length - 1)])));
			}
			else if (Unit.Army != _Army)
			{
				if (_SelectedUnit != null)
				{
					if (_AttackBuilder == null || _AttackBuilder.AttackAt != Unit.Position)
						StartAttack(new AttackOrder(_Army, Unit.Position, AttackMethod.NORMAL_FIRE));
					NoSingleAttackReason r = _AttackBuilder.AddAttacker(
						new NormalSingleAttackOrder(_SelectedUnit, Unit, AttackMethod.NORMAL_FIRE));
					if (r != NoSingleAttackReason.NONE) _GameScreen.Alert(r.ToString());
					_AttackPane.UpdateDescription();
				}
			}
		}
	}
}