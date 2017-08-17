using System;
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

		public void AddArmyView(ArmyView ArmyView)
		{
			ArmyView.UnitViews.ForEach(i => AddUnitView(i));
		}

		public void AddUnitView(UnitView UnitView)
		{
			_UnitViews.Add(UnitView.Unit, UnitView);
			UnitView.Unit.OnLoad += UpdateStack;
			UnitView.Unit.OnUnload += UpdateStack;
			UnitView.Unit.OnMove += MoveUnit;
			UnitView.Unit.OnRemove += RemoveUnit;
			if (UnitView.Unit.Position != null) MoveUnit(UnitView.Unit, UnitView.Unit.Position);
		}

		void MoveUnit(object Sender, MovementEventArgs E)
		{
			MoveUnit((Unit)Sender, E.Tile);
		}

		void MoveUnit(Unit Unit, Tile Tile)
		{
			UnitView view = _UnitViews[Unit];

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
				if (fromStack.Units.Count() == 0) _Stacks.Remove(from);
			}
		}

		void RemoveUnit(object Sender, EventArgs E)
		{
			RemoveUnit((Unit)Sender);
		}

		void RemoveUnit(Unit Unit)
		{
			KeyValuePair<Tile, StackView> from = _Stacks.FirstOrDefault(i => i.Value.Contains(Unit));
			StackView fromStack = from.Value;
			if (fromStack != null)
			{
				fromStack.Remove(Unit);
				if (fromStack.Units.Count() == 0) _Stacks.Remove(from);
			}
		}

		void UpdateStack(object Sender, EventArgs E)
		{
			UpdateStack((Unit)Sender);
		}

		void UpdateStack(Unit Unit)
		{
			KeyValuePair<Tile, StackView> from = _Stacks.FirstOrDefault(i => i.Value.Contains(Unit));
			StackView fromStack = from.Value;
			if (fromStack != null) fromStack.Sort();
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
