using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class CloseAssaultController : BaseAttackController
	{
		public CloseAssaultController(MatchAdapter Match, MatchScreen GameScreen)
			: base(Match, GameScreen)
		{
		}

		public override void HandleTileLeftClick(Tile Tile)
		{
			if (_SelectedUnit != null)
			{
				MovementOrder order = new MovementOrder(_SelectedUnit, Tile, true);
				if (_Match.ExecuteOrder(order)) SetCloseAssaultHighlight(_SelectedUnit);
				else _GameScreen.Alert(order.Validate().ToString());
			}
		}

		public override void HandleTileRightClick(Tile Tile)
		{
		}

		public override void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Army && Unit.CanAttack(AttackMethod.CLOSE_ASSAULT) == NoSingleAttackReason.NONE)
			{
				_SelectedUnit = Unit;
				SetCloseAssaultHighlight(Unit);
			}
			else if (Unit.Army != _Army)
			{
				if (_AttackBuilder == null || _AttackBuilder.AttackAt != Unit.Position)
					StartAttack(new AttackOrder(_Army, Unit.Position, AttackMethod.CLOSE_ASSAULT));
				if (_SelectedUnit != null)
				{
					NoSingleAttackReason r = _AttackBuilder.AddAttacker(
						new NormalSingleAttackOrder(_SelectedUnit, Unit, AttackMethod.CLOSE_ASSAULT));
					if (r != NoSingleAttackReason.NONE) _GameScreen.Alert(r.ToString());
					_AttackPane.UpdateDescription();
				}
			}
		}

		void SetCloseAssaultHighlight(Unit Unit)
		{
			IEnumerable<Tuple<Tile, Color>> attackRange = Unit.GetFieldOfSight(AttackMethod.CLOSE_ASSAULT).Select(
						i => new Tuple<Tile, Color>(
							i.Item1.Final,
							GetRangeColor(
								HIGHLIGHT_COLORS,
								i.Item1.Range,
								Unit.Configuration.GetRange(AttackMethod.CLOSE_ASSAULT))));

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
}