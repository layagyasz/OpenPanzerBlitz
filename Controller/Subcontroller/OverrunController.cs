using System;
using System.Linq;

using Cardamom.Graphing;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class OverrunController : BaseAttackController
	{
		Path<Tile> _InitialMovement;

		public OverrunController(Match Match, GameScreen GameScreen)
			: base(Match, GameScreen)
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
			else if (_InitialMovement == null && _SelectedUnit != null)
			{
				SetPathTo(Tile);
			}
		}

		public override void HandleTileRightClick(Tile Tile)
		{
		}

		public override void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Army && Unit.CanAttack(AttackMethod.OVERRUN) == NoSingleAttackReason.NONE)
			{
				_SelectedUnit = Unit;

				Highlight(
					Unit.GetFieldOfMovement(true).Select(
						i => new Tuple<Tile, Color>(
							i.Item1,
							HIGHLIGHT_COLORS[
								Math.Min(
									(int)Math.Ceiling(i.Item3) * 4 / Unit.Configuration.Movement,
									HIGHLIGHT_COLORS.Length - 1)])));
			}
		}

		private void DeselectUnit()
		{
			_SelectedUnit = null;
			_InitialMovement = null;
			UnHighlight();
		}

		private void FinishSingleOverrunMove(Tile Tile)
		{
			if (_SelectedUnit != null && _InitialMovement != null)
			{
				if (_AttackBuilder == null || _AttackBuilder.AttackAt != Tile)
					StartAttack(new AttackOrder(_Army, Tile, AttackMethod.OVERRUN));

				NoSingleAttackReason r =
					_AttackBuilder.AddAttacker(
						new OverrunSingleAttackOrder(
							new MovementOrder(_SelectedUnit, _InitialMovement.Destination, true), Tile));
				_AttackPane.UpdateDescription();
				if (r != NoSingleAttackReason.NONE) _GameScreen.Alert(r.ToString());
			}
			DeselectUnit();
		}

		private void SetPathTo(Tile Tile)
		{
			if (_SelectedUnit != null)
			{
				_InitialMovement = _SelectedUnit.GetPathTo(Tile, true);
				Highlight(_InitialMovement.Nodes.Select(i => new Tuple<Tile, Color>(i, HIGHLIGHT_COLORS[0])));
			}
		}
	}
}
