using System;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class EventRelay
	{
		// Unit Events
		public EventHandler<EventArgs> OnUnitLoad;
		public EventHandler<ValuedEventArgs<Unit>> OnUnitUnload;
		public EventHandler<ValuedEventArgs<Unit>> OnUnitFortify;
		public EventHandler<EventArgs> OnUnitAbandon;
		public EventHandler<ValuedEventArgs<UnitConfiguration>> OnUnitConfigurationChange;
		public EventHandler<MovementEventArgs> OnUnitMove;
		public EventHandler<EventArgs> OnUnitFire;
		public EventHandler<EventArgs> OnUnitRecover;
		public EventHandler<EventArgs> OnUnitDisrupt;
		public EventHandler<ValuedEventArgs<Tile>> OnUnitRemove;
		public EventHandler<EventArgs> OnUnitDestroy;
		public EventHandler<ValuedEventArgs<Army>> OnUnitCapture;

		// Army Events
		public EventHandler<NewUnitEventArgs> OnUnitAdded;

		public void Hook(Match Match)
		{
			foreach (var army in Match.Armies) Hook(army);
		}

		public void Hook(Army Army)
		{
			Army.OnUnitAdded += HandleUnitAdded;
			foreach (var unit in Army.Units) Hook(unit);
		}

		public void Hook(Unit Unit)
		{
			Unit.OnLoad += HandleUnitLoad;
			Unit.OnUnload += HandleUnitUnload;
			Unit.OnFortify += HandleUnitFortify;
			Unit.OnAbandon += HandleUnitAbandon;
			Unit.OnConfigurationChange += HandleUnitConfigurationChange;
			Unit.OnMove += HandleUnitMove;
			Unit.OnFire += HandleUnitFire;
			Unit.OnRecover += HandleUnitRecover;
			Unit.OnDisrupt += HandleUnitDisrupt;
			Unit.OnRemove += HandleUnitRemove;
			Unit.OnDestroy += HandleUnitDestroy;
			Unit.OnCapture += HandleUnitCapture;
		}

		void HandleUnitAdded(object Sender, NewUnitEventArgs E)
		{
			Hook(E.Unit);
			OnUnitAdded?.Invoke(Sender, E);
		}

		void HandleUnitLoad(object Sender, EventArgs E)
		{
			OnUnitLoad?.Invoke(Sender, E);
		}

		void HandleUnitFortify(object Sender, ValuedEventArgs<Unit> E)
		{
			OnUnitFortify?.Invoke(Sender, E);
		}

		void HandleUnitAbandon(object Sender, EventArgs E)
		{
			OnUnitAbandon?.Invoke(Sender, E);
		}

		void HandleUnitUnload(object Sender, ValuedEventArgs<Unit> E)
		{
			OnUnitUnload?.Invoke(Sender, E);
		}

		void HandleUnitConfigurationChange(object Sender, ValuedEventArgs<UnitConfiguration> E)
		{
			OnUnitConfigurationChange?.Invoke(Sender, E);
		}

		void HandleUnitMove(object Sender, MovementEventArgs E)
		{
			OnUnitMove?.Invoke(Sender, E);
		}

		void HandleUnitFire(object Sender, EventArgs E)
		{
			OnUnitFire?.Invoke(Sender, E);
		}

		void HandleUnitRecover(object Sender, EventArgs E)
		{
			OnUnitRecover?.Invoke(Sender, E);
		}

		void HandleUnitDisrupt(object Sender, EventArgs E)
		{
			OnUnitDisrupt?.Invoke(Sender, E);
		}

		void HandleUnitRemove(object Sender, ValuedEventArgs<Tile> E)
		{
			OnUnitRemove?.Invoke(Sender, E);
		}

		void HandleUnitDestroy(object Sender, EventArgs E)
		{
			OnUnitDestroy?.Invoke(Sender, E);
		}

		void HandleUnitCapture(object Sender, ValuedEventArgs<Army> E)
		{
			OnUnitCapture?.Invoke(Sender, E);
		}
	}
}
