﻿using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class StackLayer
	{
		Dictionary<Unit, UnitView> _UnitViews = new Dictionary<Unit, UnitView>();

		List<KeyValuePair<Tile, StackView>> _Stacks = new List<KeyValuePair<Tile, StackView>>();

		public StackLayer()
		{
		}

		public void AddArmyView(ArmyView ArmyView)
		{
			ArmyView.UnitViews.ForEach(i => AddUnitView(i));
		}

		public void AddUnitView(UnitView UnitView)
		{
			_UnitViews.Add(UnitView.Unit, UnitView);
			UnitView.Unit.OnMove += MoveUnit;
		}

		void MoveUnit(object Sender, MovementEventArgs E)
		{
			MoveUnit((Unit)Sender, E.Tile);
		}

		void MoveUnit(Unit Unit, Tile Tile)
		{
			UnitView view = _UnitViews[Unit];
			view.Position = Tile.Center;

			KeyValuePair<Tile, StackView> from = _Stacks.FirstOrDefault(i => i.Value.Contains(Unit));
			StackView fromStack = from.Value;
			StackView toStack = _Stacks.FirstOrDefault(i => i.Key == Tile).Value;

			if (toStack == null)
			{
				toStack = new StackView();
				_Stacks.Add(new KeyValuePair<Tile, StackView>(Tile, toStack));
			}
			toStack.Add(view);
			if (fromStack != null)
			{
				fromStack.Remove(Unit);
				if (fromStack.Count == 0) _Stacks.Remove(from);
			}
		}

		public void Update(
			MouseController MouseController, KeyController KeyController, int DeltaT, Transform Transform)
		{
			foreach (var s in _Stacks) s.Value.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public void Draw(RenderTarget Target, Transform Transform)
		{
			foreach (var s in _Stacks) s.Value.Draw(Target, Transform);
		}
	}
}
