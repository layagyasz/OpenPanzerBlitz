using System;
using System.Linq;

using Cardamom.Graphing;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class OverrunController : BaseAttackController
	{
		Path<Tile> _InitialMovement;

		public OverrunController(HumanMatchPlayerController Controller)
			: base(Controller)
		{
		}

		public override void HandleTileLeftClick(Tile Tile)
		{
			if (_InitialMovement != null)
			{
				if (Tile.Neighbors().Contains(_InitialMovement.Destination))
				{
					FinishSingleOverrunMove(Tile);
				}
				else SetPathTo(Tile);
			}
			else if (_InitialMovement == null && _Controller.SelectedUnit != null)
			{
				SetPathTo(Tile);
			}
		}

		public override void HandleTileRightClick(Tile Tile)
		{
		}

		public override void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Controller.CurrentTurn.Army
				&& Unit.CanAttack(AttackMethod.OVERRUN) == OrderInvalidReason.NONE)
			{
				_Controller.SelectUnit(Unit);

				_Controller.Highlight(
					Unit.GetFieldOfMovement(true).Select(
						i => new Tuple<Tile, Color>(
							i.Item1,
								HumanMatchPlayerController.HIGHLIGHT_COLORS[
								Math.Max(0, Math.Min(
									(int)(Math.Ceiling(i.Item3) * 4 / Unit.RemainingMovement),
									HumanMatchPlayerController.HIGHLIGHT_COLORS.Length - 1))])));
			}
		}

		void DeselectUnit()
		{
			_Controller.SelectUnit(null);
			_InitialMovement = null;
			_Controller.UnHighlight();
		}

		void FinishSingleOverrunMove(Tile Tile)
		{
			if (_Controller.SelectedUnit != null && _InitialMovement != null)
			{
				AddAttack(
					Tile,
					new OverrunSingleAttackOrder(
						new MovementOrder(_Controller.SelectedUnit, _InitialMovement.Destination, true),
						Tile,
						_Controller.UseSecondaryWeapon()));
			}
			DeselectUnit();
		}

		void SetPathTo(Tile Tile)
		{
			if (_Controller.SelectedUnit != null)
			{
				_InitialMovement = _Controller.SelectedUnit.GetPathTo(Tile, true);
				_Controller.Highlight(
					_InitialMovement.Nodes.Select(
						i => new Tuple<Tile, Color>(i, HumanMatchPlayerController.HIGHLIGHT_COLORS.First())));
			}
		}
	}
}
