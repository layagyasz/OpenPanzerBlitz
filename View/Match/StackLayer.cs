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

		EventBuffer<EventArgs> _EventBuffer = new EventBuffer<EventArgs>();

		public IEnumerable<UnitView> UnitViews
		{
			get
			{
				return _UnitViews.Values;
			}
		}

		public void AddUnitView(UnitView UnitView)
		{
			_UnitViews.Add(UnitView.Unit, UnitView);

			Action<object, EventArgs> updateHandler = _EventBuffer.Hook(UpdateStack);
			UnitView.Unit.OnLoad += updateHandler.Invoke;
			UnitView.Unit.OnUnload += updateHandler.Invoke;
			UnitView.Unit.OnMove += _EventBuffer.Hook((s, e) => MoveUnit(s, (MovementEventArgs)e)).Invoke;
			UnitView.Unit.OnRemove += _EventBuffer.Hook(RemoveUnit).Invoke;
			if (UnitView.Unit.Position != null)
				MoveUnit(UnitView.Unit, new MovementEventArgs(UnitView.Unit.Position, null, null));
		}

		void MoveUnit(object Sender, MovementEventArgs E)
		{
			MoveUnit((Unit)Sender, E);
		}

		void MoveUnit(Unit Unit, MovementEventArgs E)
		{
			Tile tile = E.Tile;
			if (tile != Unit.Position) return; // Received out-of-date notification.

			UnitView view = _UnitViews[Unit];
			view.Move(E);

			KeyValuePair<Tile, StackView> from = _Stacks.FirstOrDefault(i => i.Value.Contains(Unit));
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
			_EventBuffer.DispatchEvents();

			foreach (var s in _Stacks) s.Value.Update(MouseController, KeyController, DeltaT, Transform);
		}

		public void Draw(RenderTarget Target, Transform Transform)
		{
			foreach (var s in _Stacks) s.Value.Draw(Target, Transform);
		}
	}
}
