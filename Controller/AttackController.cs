﻿using System;
using System.Linq;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class AttackController : Controller
	{
		static readonly Color[] HIGHLIGHT_COLORS = new Color[] { new Color(255, 0, 0, 120), new Color(0, 255, 0, 120) };

		public AttackMethod AttackMethod;

		Army _Army;
		Match _Match;
		GameScreen _GameScreen;
		UnitConfigurationRenderer _Renderer;

		Highlight _RangeHighlight = new Highlight();

		AttackOrder _AttackBuilder;
		AttackPane _AttackPane;
		Unit _SelectedUnit;

		public AttackController(AttackMethod AttackMethod, Match Match, UnitConfigurationRenderer Renderer, GameScreen GameScreen)
		{
			this.AttackMethod = AttackMethod;

			_Match = Match;
			_GameScreen = GameScreen;
			_Renderer = Renderer;
		}

		public void Begin(Army Army)
		{
			_Army = Army;
			_GameScreen.HighlightLayer.AddHighlight(_RangeHighlight);
		}

		public void End()
		{
			_GameScreen.HighlightLayer.RemoveHighlight(_RangeHighlight);
		}

		public void HandleTileLeftClick(Tile Tile)
		{
			if (Tile.Units.All(i => i.Army != _Army) && Tile.Units.Count() > 0) StartAttack(Tile);
		}

		public void HandleTileRightClick(Tile Tile)
		{
		}

		public void HandleUnitLeftClick(Unit Unit)
		{
			if (Unit.Army == _Army)
			{
				_SelectedUnit = Unit;

				if (_AttackBuilder != null)
				{
					_AttackBuilder.AddAttacker(Unit);
					_AttackPane.UpdateDescription();
				}

				_GameScreen.HighlightLayer.RemoveHighlight(_RangeHighlight);
				_RangeHighlight = new Highlight(
					Unit.GetFieldOfSight().Select(
						i => new Tuple<Tile, Color>(
							i.Final,
							HIGHLIGHT_COLORS[
								Math.Min(
									i.Range * 2 / (Unit.UnitConfiguration.GetRange(AttackMethod) + 1),
									HIGHLIGHT_COLORS.Length - 1)])));
				_GameScreen.HighlightLayer.AddHighlight(_RangeHighlight);
			}
			else StartAttack(Unit.Position);
		}

		public void HandleUnitRightClick(Unit Unit)
		{
		}

		private void StartAttack(Tile Tile)
		{
			if (_AttackBuilder != null) _GameScreen.RemovePane(_AttackPane);

			_AttackBuilder = new AttackOrder(_Army, Tile, AttackMethod);
			if (_SelectedUnit != null) _AttackBuilder.AddAttacker(_SelectedUnit);
			_SelectedUnit = null;

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
