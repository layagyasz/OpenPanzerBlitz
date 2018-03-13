using System;
using System.Linq;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class CloseAssaultController : BaseAttackController
	{
		public CloseAssaultController(HumanMatchPlayerController Controller)
			: base(Controller) { }

		public override void HandleTileLeftClick(Tile Tile)
		{
			if (_Controller.SelectedUnit != null)
			{
				var order = new MovementOrder(_Controller.SelectedUnit, Tile, true);
				if (_Controller.ExecuteOrderAndAlert(order)) SetCloseAssaultHighlight(_Controller.SelectedUnit);
			}
		}

		public override void HandleTileRightClick(Tile Tile) { }

		public override void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Controller.CurrentTurn.Army
				&& Unit.CanAttack(AttackMethod.CLOSE_ASSAULT) == OrderInvalidReason.NONE)
			{
				_Controller.SelectUnit(Unit);
				SetCloseAssaultHighlight(Unit);
			}
			else if (Unit.Army != _Controller.CurrentTurn.Army)
			{
				if (_Controller.SelectedUnit != null)
				{
					AddAttack(
						Unit.Position,
						new CloseAssaultSingleAttackOrder(_Controller.SelectedUnit, Unit.Position));
				}
			}
		}

		void SetCloseAssaultHighlight(Unit Unit)
		{
			var attackRange = Unit.GetFieldOfSight(AttackMethod.CLOSE_ASSAULT).Select(
				i => new Tuple<Tile, Color>(
					i.Item1.Final,
					_Controller.GetRangeColor(i.Item1, Unit, i.Item2, AttackMethod.CLOSE_ASSAULT)));

			var moveRange = Unit.GetFieldOfMovement(true).Select(
				i => new Tuple<Tile, Color>(
					i.Item1,
						HumanMatchPlayerController.HIGHLIGHT_COLORS[
						Math.Min(
							(int)(Math.Ceiling(i.Item3) * 4 / Unit.RemainingMovement),
							HumanMatchPlayerController.HIGHLIGHT_COLORS.Length - 1)]));

			_Controller.Highlight(attackRange.Concat(moveRange.Where(i => !attackRange.Any(j => j.Item1 == i.Item1))));
		}
	}
}
