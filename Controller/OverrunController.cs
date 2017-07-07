using System;
using System.Linq;

using Cardamom.Graphing;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class OverrunController : Controller
	{
		static readonly Color[] HIGHLIGHT_COLORS = new Color[]
		{
			new Color(255, 0, 0, 120),
			new Color(255, 128, 0, 120),
		  	new Color(255, 255, 0, 120),
			new Color(0, 255, 0, 120)
		};

		Army _Army;

		Match _Match;
		GameScreen _GameScreen;

		OverrunAttackOrder _AttackBuilder;
		AttackPane _AttackPane;

		Unit _SelectedUnit;
		Path<Tile> _InitialMovement;

		Highlight _MoveHighlight;

		public OverrunController(Match Match, GameScreen GameScreen)
		{
			_Match = Match;
			_GameScreen = GameScreen;
		}

		public void Begin(Army Army)
		{
			_Army = Army;

			_MoveHighlight = new Highlight();
			_GameScreen.HighlightLayer.AddHighlight(_MoveHighlight);
		}

		public void End()
		{
			_GameScreen.HighlightLayer.RemoveHighlight(_MoveHighlight);
		}

		public void HandleTileLeftClick(Tile Tile)
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

		public void HandleTileRightClick(Tile Tile)
		{
		}

		public void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Army && Unit.CanAttack(AttackMethod.OVERRUN) == NoSingleAttackReason.NONE)
			{
				_SelectedUnit = Unit;

				_GameScreen.HighlightLayer.RemoveHighlight(_MoveHighlight);
				_MoveHighlight = new Highlight(
					Unit.GetFieldOfMovement(true).Select(
						i => new Tuple<Tile, Color>(
							i.Item1,
							HIGHLIGHT_COLORS[
								Math.Min(
									(int)Math.Ceiling(i.Item3) * 4 / Unit.UnitConfiguration.Movement,
									HIGHLIGHT_COLORS.Length - 1)])));
				_GameScreen.HighlightLayer.AddHighlight(_MoveHighlight);
			}
		}

		public void HandleUnitRightClick(Unit Unit)
		{
		}

		private void UnHighlight()
		{
			_GameScreen.HighlightLayer.RemoveHighlight(_MoveHighlight);
			_MoveHighlight = new Highlight();
			_GameScreen.HighlightLayer.AddHighlight(_MoveHighlight);
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
				if (_AttackBuilder == null) StartAttack(Tile);
				_AttackBuilder.AddAttacker(
					_SelectedUnit,
					new OverrunMoveOrder(new MovementOrder(_SelectedUnit, _InitialMovement.Destination, true), Tile));
				_AttackPane.UpdateDescription();
			}
			DeselectUnit();
		}

		private void SetPathTo(Tile Tile)
		{
			if (_SelectedUnit != null)
			{
				_InitialMovement = _SelectedUnit.GetPathTo(Tile, true);
				_GameScreen.HighlightLayer.RemoveHighlight(_MoveHighlight);
				_MoveHighlight = new Highlight(
					_InitialMovement.Nodes.Select(i => new Tuple<Tile, Color>(i, HIGHLIGHT_COLORS[0])));
				_GameScreen.HighlightLayer.AddHighlight(_MoveHighlight);
			}
		}

		private void StartAttack(Tile Tile)
		{
			if (_AttackBuilder != null) _GameScreen.RemovePane(_AttackPane);

			_AttackBuilder = new OverrunAttackOrder(_Army, Tile);

			_AttackPane = new AttackPane(_AttackBuilder);
			_GameScreen.AddPane(_AttackPane);
			_AttackPane.UpdateDescription();
			_AttackPane.OnExecute += ExecuteAttack;
		}

		private void ExecuteAttack(object sender, EventArgs e)
		{
			if (_Match.ExecuteOrder(_AttackBuilder)) _GameScreen.RemovePane(_AttackPane);
			else _AttackPane.UpdateDescription();
		}
	}
}
