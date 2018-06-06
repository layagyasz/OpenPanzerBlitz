using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;

using SFML.Graphics;

namespace PanzerBlitz
{
	public class StackLayer : Pod
	{
		static byte LAST_SEEN_ALPHA = 128;

		Dictionary<Unit, UnitView> _UnitViews = new Dictionary<Unit, UnitView>();

		readonly List<KeyValuePair<Tile, StackView>> _Stacks = new List<KeyValuePair<Tile, StackView>>();

		public IEnumerable<UnitView> UnitViews
		{
			get
			{
				return _UnitViews.Values;
			}
		}

		public void SetUnitVisibilities(SightFinder SightFinder)
		{
			foreach (var unit in _UnitViews) SetUnitVisibility(unit.Key, SightFinder.GetUnitVisibility(unit.Key));
		}

		public void UpdateUnitVisibilities(
			Unit Unit, MovementEventArgs Movement, List<Tuple<Unit, UnitVisibility>> UnitDeltas)
		{
			foreach (var delta in UnitDeltas) SetUnitVisibility(delta.Item1, delta.Item2);
			if (Unit != null && Movement != null) MoveUnit(Unit, Movement, true);
			else if (Unit != null) UpdateStack(Unit);
		}

		public void AddUnitView(UnitView UnitView, SightFinder SightFinder)
		{
			_UnitViews.Add(UnitView.Unit, UnitView);
			if (SightFinder != null) SetUnitVisibility(UnitView.Unit, SightFinder.GetUnitVisibility(UnitView.Unit));
		}

		void SetUnitVisibility(Unit Unit, UnitVisibility Visibility)
		{
			if (Visibility.LastSeen == null) RemoveUnit(Unit);
			else MoveUnit(Unit, new MovementEventArgs(Visibility.LastSeen, null, null), Visibility.Visible);
		}

		void MoveUnit(Unit Unit, MovementEventArgs E, bool Visible)
		{
			Tile tile = E.Tile;

			UnitView view = _UnitViews[Unit];
			view.Move(E);
			view.SetAlpha(Visible ? (byte)255 : LAST_SEEN_ALPHA);

			var from = _Stacks.FirstOrDefault(i => i.Value.Contains(Unit));
			StackView fromStack = from.Value;
			StackView toStack = _Stacks.FirstOrDefault(i => i.Key == tile).Value;

			if (toStack != null && toStack == fromStack) return;
			if (toStack == null)
			{
				toStack = new StackView();
				_Stacks.Add(new KeyValuePair<Tile, StackView>(tile, toStack));
			}
			toStack.Add(view);
			if (fromStack != null)
			{
				fromStack.Remove(Unit);
				if (fromStack.Units.Count() == 0) _Stacks.Remove(from);
			}
		}

		void RemoveUnit(Unit Unit)
		{
			var from = _Stacks.FirstOrDefault(i => i.Value.Contains(Unit));
			StackView fromStack = from.Value;
			if (fromStack != null)
			{
				fromStack.Remove(Unit);
				if (fromStack.Units.Count() == 0) _Stacks.Remove(from);
			}
		}

		void UpdateStack(Unit Unit)
		{
			var from = _Stacks.FirstOrDefault(i => i.Value.Contains(Unit));
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
