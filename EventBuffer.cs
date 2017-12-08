using System;
using System.Collections.Generic;

namespace PanzerBlitz
{
	public class EventBuffer<T> where T : EventArgs
	{
		Action<object, T> _Method;
		Queue<Tuple<object, T>> _Invocations = new Queue<Tuple<object, T>>();

		public EventBuffer(Action<object, T> Method)
		{
			_Method = Method;
		}

		public void QueueEvent(object Sender, T E)
		{
			lock (_Invocations)
			{
				_Invocations.Enqueue(new Tuple<object, T>(Sender, E));
			}
		}

		public void DispatchEvents()
		{
			lock (_Invocations)
			{
				foreach (Tuple<object, T> args in _Invocations) _Method(args.Item1, args.Item2);
				_Invocations.Clear();
			}
		}
	}
}
