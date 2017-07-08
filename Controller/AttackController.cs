using System;
using System.Linq;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class AttackController : BaseAttackController<AttackOrder>
	{
		public AttackController(Match Match, GameScreen GameScreen)
			: base(Match, GameScreen)
		{
		}

		public override void HandleTileLeftClick(Tile Tile)
		{
			if (Tile.Units.All(i => i.Army != _Army) && Tile.Units.Count() > 0)
			{
				StartAttack(new AttackOrder(_Army, Tile, AttackMethod.NORMAL_FIRE));
				if (_SelectedUnit != null)
				{
					_AttackBuilder.AddAttacker(_SelectedUnit);
					_AttackPane.UpdateDescription();
				}
			}
		}

		public override void HandleTileRightClick(Tile Tile)
		{
		}

		public override void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Army)
			{
				_SelectedUnit = Unit;

				if (_AttackBuilder != null)
				{
					_AttackBuilder.AddAttacker(Unit);
					_AttackPane.UpdateDescription();
				}

				Highlight(
					Unit.GetFieldOfSight(AttackMethod.NORMAL_FIRE).Select(
						i => new Tuple<Tile, Color>(
							i.Final,
							HIGHLIGHT_COLORS[
								Math.Min(
									i.Range * HIGHLIGHT_COLORS.Length
									/ (Unit.UnitConfiguration.GetRange(AttackMethod.NORMAL_FIRE) + 1),
									HIGHLIGHT_COLORS.Length - 1)])));
			}
			else
			{
				StartAttack(new AttackOrder(_Army, Unit.Position, AttackMethod.NORMAL_FIRE));
				if (_SelectedUnit != null)
				{
					_AttackBuilder.AddAttacker(_SelectedUnit);
					_AttackPane.UpdateDescription();
				}
			}
		}

		public override void HandleUnitRightClick(Unit Unit)
		{
		}
	}
}
