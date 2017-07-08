using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class CloseAssaultController : BaseAttackController<AttackOrder>
	{
		public CloseAssaultController(Match Match, GameScreen GameScreen)
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
			if (Unit.Army == _Army && Unit.CanAttack(AttackMethod.CLOSE_ASSAULT) == NoSingleAttackReason.NONE)
			{
				_SelectedUnit = Unit;

				IEnumerable<Tuple<Tile, Color>> attackRange = Unit.GetFieldOfSight(AttackMethod.CLOSE_ASSAULT).Select(
										i => new Tuple<Tile, Color>(
											i.Final,
											HIGHLIGHT_COLORS[
												Math.Min(
													i.Range * HIGHLIGHT_COLORS.Length
								/ (Unit.UnitConfiguration.GetRange(AttackMethod.CLOSE_ASSAULT) + 1),
												HIGHLIGHT_COLORS.Length - 1)]));

				IEnumerable<Tuple<Tile, Color>> moveRange = Unit.GetFieldOfMovement(true).Select(
										i => new Tuple<Tile, Color>(
											i.Item1,
											HIGHLIGHT_COLORS[
												Math.Min(
													(int)(Math.Ceiling(i.Item3) * 4 / Unit.RemainingMovement),
												HIGHLIGHT_COLORS.Length - 1)]));

				Highlight(attackRange.Concat(moveRange.Where(i => !attackRange.Any(j => j.Item1 == i.Item1))));
			}
		}

		public override void HandleUnitRightClick(Unit Unit)
		{
		}
	}
}
