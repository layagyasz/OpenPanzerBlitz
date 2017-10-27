using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class CloseAssaultController : BaseAttackController
	{
		public CloseAssaultController(HumanMatchPlayerController Controller)
			: base(Controller)
		{
		}

		public override void HandleTileLeftClick(Tile Tile)
		{
			if (_Controller.SelectedUnit != null)
			{
				MovementOrder order = new MovementOrder(_Controller.SelectedUnit, Tile, true);
				if (_Controller.Match.ExecuteOrder(order)) SetCloseAssaultHighlight(_Controller.SelectedUnit);
				else _Controller.Alert(order.Validate());
			}
		}

		public override void HandleTileRightClick(Tile Tile)
		{
		}

		public override void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Controller.CurrentTurn.Army
				&& Unit.CanAttack(AttackMethod.CLOSE_ASSAULT) == NoSingleAttackReason.NONE)
			{
				_Controller.SelectUnit(Unit);
				SetCloseAssaultHighlight(Unit);
			}
			else if (Unit.Army != _Controller.CurrentTurn.Army)
			{
				if (_AttackBuilder == null || _AttackBuilder.AttackAt != Unit.Position)
					StartAttack(
						new AttackOrder(_Controller.CurrentTurn.Army, Unit.Position, AttackMethod.CLOSE_ASSAULT));
				if (_Controller.SelectedUnit != null)
				{
					NoSingleAttackReason r = _AttackBuilder.AddAttacker(
						new NormalSingleAttackOrder(_Controller.SelectedUnit, Unit, AttackMethod.CLOSE_ASSAULT));
					if (r != NoSingleAttackReason.NONE) _Controller.Alert(r);
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

			_Controller.Highlight(attackRange.Concat(moveRange.Where(i => !attackRange.Any(j => j.Item1 == i.Item1))));
		}
	}
}
